using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Dapper.Helper;
using WeigthIndicator.Domain.DTO;
using WeigthIndicator.Domain.Exceptions;
using WeigthIndicator.Domain.Models;
using WeigthIndicator.Domain.Services;

namespace WeigthIndicator.Dapper.Services
{
    public class ReestrDataService : IReestrDataService
    {
        public const ReestrLocationState EveryWhere = ReestrLocationState.InWarehouse | ReestrLocationState.Outcomed | ReestrLocationState.RetrunedBack;

        private string reestrSelectQuery = $@"SELECT *FROM Reestrs as reestr 
                                Left join Recipes as recipe 
                                on recipe.id = reestr.recipeid  
                                Left join BarrelStorages as br  
                                on reestr.barrelStorageId =br.id 
                                Left join Customers as c 
                                on reestr.customerid =c.id
                                Left join Manufactures as m 
                                on reestr.manufactureId =m.id ";


        private readonly ApplicationContextFactory _factory;

        public ReestrDataService(ApplicationContextFactory applicationContextFactory)
        {
            _factory = applicationContextFactory;
        }


        public async Task<Reestr> CreateReestr(Reestr reestr)
        {
            using (var connection = _factory.CreateConnection())
            {
                int barrelNumber = 1;
                string firstQuery = "SELECT * FROM Reestrs ORDER BY id DESC LIMIT 1";

                var result = await connection.QueryFirstOrDefaultAsync<Reestr>(firstQuery);

                if (result != null)
                {
                    barrelNumber = result.BarrelNumber + 1;
                }
                reestr.BarrelNumber = barrelNumber;
                await connection.InsertAsync(reestr);
                return reestr;

            }
        }
        public async Task<IEnumerable<Reestr>> GetReestrsByDate(DateTime dateTime,
            ReestrLocationState reestrLocationState = ReestrLocationState.InWarehouse | ReestrLocationState.Outcomed | ReestrLocationState.RetrunedBack)
        {
            using (var connection = _factory.CreateConnection())
            {
                string query = reestrSelectQuery + "where date(reestr.packingDate) =@date ";
                if (reestrLocationState != EveryWhere)
                {
                    query += " and reestr.reestrLocationState in @locations";
                }
                return await connection.QueryAsync<Reestr, Recipe, BarrelStorage, Customer, Manufacture,Reestr>(query,
                    (reestr, recipe, br, c,m) =>
                    {
                        reestr.Recipe = recipe;
                        reestr.BarrelStorage = br;
                        reestr.Customer = c;
                        reestr.Manufacture = m;
                        return reestr;
                    }
                    , new
                    {
                        date = dateTime.Date,
                        locations = reestrLocationState.GetUniqueFlags(),
                    });
            }
        }

        public async Task<IEnumerable<Reestr>> GetReestrsByDateAndType(DateTime dateTime, int type,
            ReestrLocationState reestrLocationState = ReestrLocationState.InWarehouse | ReestrLocationState.Outcomed | ReestrLocationState.RetrunedBack)
        {
            using (var connection = _factory.CreateConnection())
            {
                string query = reestrSelectQuery + "where date(reestr.packingDate) =@date";

                if (type != -1)
                {
                    query += " and recipe.BarrelRecipeType =@type";
                }
                if (reestrLocationState != EveryWhere)
                {
                    query += " and reestr.reestrLocationState in @locations";
                }
                return await connection.QueryAsync<Reestr, Recipe, BarrelStorage, Customer, Manufacture,Reestr>(query,
                    (reestr, recipe, br, c,m) =>
                    {
                        reestr.Recipe = recipe;
                        reestr.BarrelStorage = br;
                        reestr.Customer = c;
                        reestr.Manufacture = m;
                        return reestr;
                    }
                    , new
                    {
                        date = dateTime.Date,
                        type,
                        locations = reestrLocationState.GetUniqueFlags()
                    });
            }
        }

        public async Task<Reestr> CreateReestrAndUpdateBarrelStorage(Reestr reestr)
        {
            using (var connection = _factory.CreateConnection())
            {
                bool isNextBarrelExist = true;
                string barrelQuery = "SELECT *FROM BarrelStorages where isEmpty = false and recipeId =@recipeId";

                var barrelStorage = await connection.QueryFirstOrDefaultAsync<BarrelStorage>(barrelQuery, new { recipeId = reestr.RecipeId });

                if (barrelStorage == null)
                {
                    throw new BarrelStorageEmptyException();
                }

                if (barrelStorage.ConsumptionWeight + reestr.Net > barrelStorage.TotalWeight)
                {

                    barrelQuery += " and id != @id";

                    var nextBarrel = await connection.QueryFirstOrDefaultAsync<BarrelStorage>(barrelQuery, new { recipeId = reestr.RecipeId, id = barrelStorage.Id });
                    if (nextBarrel != null)
                    {
                        var reminder = barrelStorage.TotalWeight - barrelStorage.ConsumptionWeight;
                        nextBarrel.TotalWeight += reminder;
                        barrelStorage.TotalWeight -= reminder;

                        string emptyUpdateQuery = "UPDATE BarrelStorages Set isEmpty= true,totalWeight = @TotalWeight  WHERE id =@id";
                        await connection.QueryAsync(emptyUpdateQuery, new { id = barrelStorage.Id, totalWeight = barrelStorage.TotalWeight });
                        barrelStorage = nextBarrel;
                    }
                    else
                    {
                        isNextBarrelExist = false;
                    }


                }
                if (isNextBarrelExist)
                    barrelStorage.ConsumptionWeight += reestr.Net;


                await connection.UpdateAsync(barrelStorage);

                reestr.BarrelStorageId = barrelStorage.Id;
                reestr.BarrelStorage = barrelStorage;


                int barrelNumber = 1;
                string firstQuery = @"SELECT *FROM Reestrs as r
                                    LEFT JOIN Recipes as rc
                                    on rc.id = r.recipeid
                                    WHERE rc.BarrelRecipeType = @type
                                    ORDER BY r.id DESC LIMIT 1";

                var result = await connection.QueryFirstOrDefaultAsync<Reestr>(firstQuery, new { type = reestr.Recipe.BarrelRecipeType });

                if (result != null)
                {
                    barrelNumber = result.BarrelNumber + 1;
                }
                if (reestr.BarrelNumber != 0)
                {
                    barrelNumber = reestr.BarrelNumber;
                }
                reestr.BarrelNumber = barrelNumber;
                await connection.InsertAsync(reestr);
                return reestr;

            }
        }


        public async Task<bool> UpdateReestr(Reestr reestr)
        {
            using (var connection = _factory.CreateConnection())
            {
                var isSuccess = await connection.UpdateAsync(reestr);
                return isSuccess;
            }
        }

        public async Task<IEnumerable<Reestr>> GetReestrsByDates(DateTime fromDate, DateTime toDate, int type,
            ReestrLocationState reestrLocationState = ReestrLocationState.InWarehouse | ReestrLocationState.Outcomed | ReestrLocationState.RetrunedBack)
        {
            using (var connection = _factory.CreateConnection())
            {
                string query = reestrSelectQuery + "where (date(reestr.packingDate) >=@fromDate and date(reestr.packingDate) <=@toDate)";
                if (type != -1)
                {
                    query += " and recipe.BarrelRecipeType =@type";
                }
                if (reestrLocationState != EveryWhere)
                {
                    query += " and reestr.reestrLocationState in @locations";
                }
                return await connection.QueryAsync<Reestr, Recipe, BarrelStorage, Customer, Manufacture,Reestr>(query,
                    (reestr, recipe, br, c,m) =>
                    {
                        reestr.Recipe = recipe;
                        reestr.BarrelStorage = br;
                        reestr.Customer = c;
                        reestr.Manufacture = m;
                        return reestr;
                    }
                    , new
                    {
                        fromDate = fromDate.Date,
                        toDate = toDate.Date,
                        type,
                        locations = reestrLocationState.GetUniqueFlags()
                    });
            }
        }

        public async Task<IEnumerable<Reestr>> GetReestrsByBatchNumber(DateTime fromDate, DateTime toDate, string batchNumber, int type,
            ReestrLocationState reestrLocationState = ReestrLocationState.InWarehouse | ReestrLocationState.Outcomed | ReestrLocationState.RetrunedBack)
        {
            using (var connection = _factory.CreateConnection())
            {
                string query = reestrSelectQuery + "where (date(reestr.packingDate) >=@fromDate and date(reestr.packingDate) <=@toDate) and reestr.batchNumber=@batchNumber";


                if (type != -1)
                {
                    query += " and recipe.BarrelRecipeType =@type";
                }
                if (reestrLocationState != EveryWhere)
                {
                    query += " and reestr.reestrLocationState in @locations";
                }

                return await connection.QueryAsync<Reestr, Recipe, BarrelStorage, Customer, Manufacture,Reestr>(query,
                    (reestr, recipe, br, c,m) =>
                    {
                        reestr.Recipe = recipe;
                        reestr.BarrelStorage = br;
                        reestr.Customer = c;
                        reestr.Manufacture = m;
                        return reestr;
                    }
                    , new
                    {
                        fromDate = fromDate.Date,
                        toDate = toDate.Date,
                        batchNumber,
                        type,
                        locations = reestrLocationState.GetUniqueFlags(),
                    });
            }
        }

        public async Task<IEnumerable<Reestr>> GetReestrsByBarrelNumbers(DateTime fromDate, DateTime toDate, int from, int to, int type,
            ReestrLocationState reestrLocationState = ReestrLocationState.InWarehouse | ReestrLocationState.Outcomed | ReestrLocationState.RetrunedBack)
        {
            using (var connection = _factory.CreateConnection())
            {
                string query = reestrSelectQuery + "where (date(reestr.packingDate) >=@fromDate and date(reestr.packingDate) <=@toDate) and reestr.barrelNumber >=@from and reestr.barrelNumber <=@to";

                if (type != -1)
                {
                    query += " and recipe.BarrelRecipeType =@type";
                }
                if(reestrLocationState != EveryWhere)
                {
                    query += " and reestr.reestrLocationState in @locations";
                }

                return await connection.QueryAsync<Reestr, Recipe, BarrelStorage, Customer, Manufacture,Reestr>(query,
                    (reestr, recipe, br, c,m) =>
                    {
                        reestr.Recipe = recipe;
                        reestr.BarrelStorage = br;
                        reestr.Customer = c;
                        reestr.Manufacture = m;
                        return reestr;
                    }
                    , new
                    {
                        fromDate = fromDate.Date,
                        toDate = toDate.Date,
                        from,
                        to,
                        type,
                        locations = reestrLocationState.GetUniqueFlags(),
                    });
            }
        }

        public async Task<IEnumerable<GroupedReestr>> GetGroupedReestrs(DateTime fromDate, DateTime toDate)
        {
            using (var connection = _factory.CreateConnection())
            {
                string query = @"SELECT reestr.BatchNumber,recipe.ShortName as RecipeName,Count(recipe.id) as barrelscount,Sum(reestr.Net) as totalnet FROM Reestrs as reestr 
                                              Left join Recipes as recipe
                                              on recipe.id = reestr.recipeid
                                              where(date(reestr.packingDate) >= @fromDate and date(reestr.packingDate) <= @toDate)
                                              group by recipe.ShortName,reestr.BatchNumber;";


                return await connection.QueryAsync<GroupedReestr>(query,
                    new
                    {
                        fromDate = fromDate.Date,
                        toDate = toDate.Date,

                    });
            }
        }

        public async Task<IEnumerable<GroupedReestr>> GetGroupedReestrs(DateTime fromDate)
        {
            using (var connection = _factory.CreateConnection())
            {
                string query = @"SELECT reestr.BatchNumber,recipe.ShortName as RecipeName,Count(recipe.id) as barrelscount,Sum(reestr.Net) as totalnet FROM Reestrs as reestr 
                                              Left join Recipes as recipe
                                              on recipe.id = reestr.recipeid
                                              where date(reestr.packingDate) >= @fromDate
                                              group by recipe.ShortName,reestr.BatchNumber;";


                return await connection.QueryAsync<GroupedReestr>(query,
                    new
                    {
                        fromDate = fromDate.Date,
                    });
            }
        }

        public async Task<Reestr> GetLastReestr(int recipeId, DateTime date)
        {
            using (var connection = _factory.CreateConnection())
            {
                string query = "SELECT *FROM Reestrs where recipeid =@recipeId and Date(PackingDate) =@date  order by id desc";

                return await connection.QueryFirstOrDefaultAsync<Reestr>(query, new { recipeId, date = date.Date });
            }
        }

        public async Task<IEnumerable<Reestr>> FilterReestr(
            DateTime packageDate, DateTime productionDate,
            string barrelNumber = null,
            int recipeId = 0,
            string batch = "Все",
            BarrelRecipeType? barrelRecipeType = null)
        {
            using (var connection = _factory.CreateConnection())
            {
                var filterQuery = @"Select *from Reestrs as reestr
                                   Left join Recipes as r on reestr.RecipeId = r.Id
                                   Left join BarrelStorages as bs on reestr.BarrelStorageId = bs.Id
                                   where";
                List<string> subQuery = new List<string>();

                bool and = false;
                if (packageDate != DateTime.MinValue)
                {
                    subQuery.Add(" Date(reestr.packingDate) =Date(@packageDate) ");
                }
                if (productionDate != DateTime.MinValue)
                {
                    subQuery.Add(" Date(bs.productionDate) =Date(@productionDate) ");
                }
                if (!string.IsNullOrWhiteSpace(barrelNumber))
                {
                    subQuery.Add(" reestr.BarrelNumber=@barrelNumber ");
                }
                if (recipeId != 0)
                {
                    subQuery.Add(" reestr.RecipeId=@recipeId ");
                }
                if (batch != "Все" && batch != null)
                {
                    subQuery.Add(" reestr.BatchNumber=@batch ");                
                }
                if (barrelRecipeType != null)
                {
                    subQuery.Add(" r.BarrelRecipeType=@barrelRecipeType");                  
                }
                if(subQuery.Count == 0)
                {
                    return Enumerable.Empty<Reestr>();
                    //filterQuery = filterQuery.Replace("where", "");
                }
                var query = string.Join("and", subQuery);
                return await connection.QueryAsync<Reestr, Recipe, BarrelStorage,Reestr>(filterQuery + query,
                      (reestr,recipe,bs) =>
                      {
                          reestr.Recipe = recipe;
                          reestr.BarrelStorage = bs;
                          return reestr;
                      },
                      new { packageDate, productionDate, barrelNumber, recipeId, batch, barrelRecipeType });

            }
        }
    }
}
