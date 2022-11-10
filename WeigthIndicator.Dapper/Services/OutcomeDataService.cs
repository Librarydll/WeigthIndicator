using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Domain;
using WeigthIndicator.Domain.Exceptions;
using WeigthIndicator.Domain.Models;
using WeigthIndicator.Domain.Services;

namespace WeigthIndicator.Dapper.Services
{
    public class OutcomeDataService : IOutcomeDataService
    {
        private readonly ApplicationContextFactory _factory;

        private string reestrSelectQuery =
                              $@"SELECT *FROM Reestrs as reestr 
                                Left join Recipes as recipe 
                                on recipe.id = reestr.recipeid  
                                Left join BarrelStorages as br  
                                on reestr.barrelStorageId =br.id ";

        public OutcomeDataService(ApplicationContextFactory factory)
        {
            _factory = factory;
        }
        public async Task<Outcome> CreateOutcome(Outcome outcome)
        {
            using (var connection = _factory.CreateConnection())
            {
                string checkQuery = reestrSelectQuery + " where  reestr.id=@ReestrId";
                foreach (var outcomeItem in outcome.OutcomeItems)
                {
                    var existed = await connection.QueryAsync<Reestr, Recipe, BarrelStorage, Reestr>(checkQuery,
                        (reestr, recipe, br) =>
                        {
                            reestr.Recipe = recipe;
                            reestr.BarrelStorage = br;
                            return reestr;

                        }, new { ReestrId =outcomeItem.ReestrId });

                    var first = existed.FirstOrDefault();
                    if (first !=null && first.ReestrLocationState == ReestrLocationState.Outcomed)
                    {
                        throw new ReestrAlreadyOutcomedException(first);
                    }
                }


                var insertOutcomeQuery = "Insert into outcomes (Title,Comment,Created,UserId) VALUES(@Title,@Comment,@Created,@UserId)";
                var getlastIdQuery = "SELECT LAST_INSERT_ID();";

                await connection.ExecuteAsync(insertOutcomeQuery, outcome);
                outcome.Id = await connection.QueryFirstOrDefaultAsync<int>(getlastIdQuery);

                var insertOutcomeitemQuery = "Insert into outcomeitems (Count,Comment,ReestrId,OutcomeId) VALUES (@Count,@Comment,@ReestrId,@OutcomeId)";
                var reestrStateChangedToOutcomeQuery = "Update reestrs set ReestrLocationState = 1 where id=@id";

                foreach (var item in outcome.OutcomeItems)
                {
                    item.OutcomeId = outcome.Id;
                    await connection.ExecuteAsync(insertOutcomeitemQuery, item);
                    await connection.ExecuteAsync(reestrStateChangedToOutcomeQuery, new { id = item.ReestrId });
                }


                return outcome;
            }
        }

        public async Task<bool> DeleteOutcome(int outcomeId)
        {
            using (var connection = _factory.CreateConnection())
            {
                var outcomeItemsQeury = "Update reestrs set ReestrLocationState = 0 where id in( Select reestrid from outcomeItems where outcomeid =@outcomeId)";
                await connection.ExecuteAsync(outcomeItemsQeury, new { outcomeId });

                var removeQuery = "Delete from Outcomes where id=@id";
                await connection.ExecuteAsync(removeQuery, new { id = outcomeId });

                return true;
            }
        }

        public async Task<Outcome> GetOutcome(int id)
        {
            using (var connection = _factory.CreateConnection())
            {
                var query = @"Select *from outcomes as outcome 
                            left join outcomeitems as oitem on outcome.Id =oitem.Outcomeid
                            left join reestrs as r on oitem.ReestrId = r.id
                            where outcome.Id =@id";

                Outcome current = null;

                var outcomes = await connection.QueryAsync<Outcome, OutcomeItem, Reestr, Outcome>(query,
                      (outcome, oitem, reestr) =>
                      {
                          if (current == null) current = outcome;

                          current.OutcomeItems = current.OutcomeItems ?? new List<OutcomeItem>();
                          oitem.Reestr = reestr;
                          current.OutcomeItems.Add(oitem);
                          return current;
                      },
                      new
                      {
                          id
                      });

                return outcomes.FirstOrDefault();
            }
        }

        public async Task<PaginationObject<Outcome>> GetOutcomes(DateTime from, DateTime to, int currentPage, int pageSize, string searchingString)
        {
            using (var connection = _factory.CreateConnection())
            {
                var currentP = currentPage - 1;
                var subQuery = "where Date(Created) >= Date(@from) and Date(Created) <= Date(@to) ";
                var query = @"Select *from Outcomes ";
                if (!string.IsNullOrWhiteSpace(searchingString))
                {
                    subQuery += "and title like '%@searchingString%'";
                }
                query += subQuery;
                var countQuery = "Select Count(*) from Outcomes " + subQuery;
                var par = new { from, to, currentP, pageSize, searchingString };
                var items = await connection.QueryAsync<Outcome>(query + " Limit @currentP,@pageSize", par);
                var count = await connection.QueryFirstOrDefaultAsync<int>(countQuery, par);

                return new PaginationObject<Outcome>
                {
                    Collection = items,
                    ItemsCount = count
                };
            }
        }

        public async Task<Outcome> UpdateOutcome(Outcome outcome)
        {
            using (var connection = _factory.CreateConnection())
            {

                var updateOutcomeQuery = "Update outcomes set Comment=@Comment ,Created=@Created Where Id=@Id";
                await connection.ExecuteAsync(updateOutcomeQuery, outcome);

                var insertOutcomeitemQuery = "Insert into outcomeitems (Count,Comment,ReestrId,OutcomeId) VALUES (@Count,@Comment,@ReestrId,@OutcomeId)";
                var reestrStateChangedToOutcomeQuery = "Update reestrs set ReestrLocationState = 1 where id=@id";

                var reestrStateChangedToWarehouseQuery = "Update reestrs set ReestrLocationState = 0 where id=@id";


                var updateOutcomeitemQuery = "Update outcomeItems Set Count=@Count,Comment=@Comment where id=@id";
                var deleteOutcomeitemQuery = "Delete from outcomeItems where id=@id";

                foreach (var item in outcome.OutcomeItems)
                {
                    if (item.Id == 0)
                    {
                        item.OutcomeId = outcome.Id;
                        await connection.ExecuteAsync(insertOutcomeitemQuery, item);
                        await connection.ExecuteAsync(reestrStateChangedToOutcomeQuery, new { id = item.ReestrId });
                    }
                    else
                    {
                        if (item.Count == -1)
                        {
                            await connection.ExecuteAsync(deleteOutcomeitemQuery, item);
                            await connection.ExecuteAsync(reestrStateChangedToWarehouseQuery, new { id = item.ReestrId });
                        }
                        else
                        {
                            await connection.ExecuteAsync(updateOutcomeitemQuery, item);
                        }
                    }
                }


                return outcome;
            }
        }
    }
}
