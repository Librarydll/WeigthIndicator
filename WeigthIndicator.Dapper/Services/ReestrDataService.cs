using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Domain.Exceptions;
using WeigthIndicator.Domain.Models;
using WeigthIndicator.Domain.Services;

namespace WeigthIndicator.Dapper.Services
{
    public class ReestrDataService : IReestrDataService
    {
        private string reestrSelectQuery = @"SELECT *FROM Reestrs as reestr " +
                                "Left join Recipes as recipe " +
                                "on recipe.id = reestr.recipeid " +
                                "Left join BarrelStorages as br " +
                                "on reestr.barrelStorageId =br.id " +
                                "Left join Customers as c " +
                                "on reestr.customerid =c.id ";

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

        public async Task<IEnumerable<Reestr>> GetReestrsByDate(DateTime dateTime)
        {
            using (var connection = _factory.CreateConnection())
            {
                string query = reestrSelectQuery + "where date(reestr.packingDate) =@date";

                return await connection.QueryAsync<Reestr, Recipe, BarrelStorage, Customer, Reestr>(query,
                    (reestr, recipe, br, c) =>
                    {
                        reestr.Recipe = recipe;
                        reestr.BarrelStorage = br;
                        reestr.Customer = c;
                        return reestr;
                    }
                    , new
                    {
                        date = dateTime.Date
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


        public async Task<bool> UpdateReestr(Reestr reestr)
        {
            using (var connection = _factory.CreateConnection())
            {
                var isSuccess = await connection.UpdateAsync(reestr);
                return isSuccess;
            }
        }

        public async Task<IEnumerable<Reestr>> GetReestrsByDates(DateTime fromDate, DateTime toDate)
        {
            using (var connection = _factory.CreateConnection())
            {
                string query = reestrSelectQuery + "where date(reestr.packingDate) >=@fromDate and date(reestr.packingDate) <=@toDate";

                return await connection.QueryAsync<Reestr, Recipe, BarrelStorage, Customer, Reestr>(query,
                    (reestr, recipe, br, c) =>
                    {
                        reestr.Recipe = recipe;
                        reestr.BarrelStorage = br;
                        reestr.Customer = c;
                        return reestr;
                    }
                    , new
                    {
                        fromDate = fromDate.Date,
                        toDate = toDate.Date
                    });
            }
        }

        public async Task<IEnumerable<Reestr>> GetReestrsByBatchNumber(string batchNumber)
        {
            using (var connection = _factory.CreateConnection())
            {
                string query = reestrSelectQuery + "where reestr.batchNumber=@batchNumber";

                return await connection.QueryAsync<Reestr, Recipe, BarrelStorage, Customer, Reestr>(query,
                    (reestr, recipe, br, c) =>
                    {
                        reestr.Recipe = recipe;
                        reestr.BarrelStorage = br;
                        reestr.Customer = c;
                        return reestr;
                    }
                    , new
                    {
                        batchNumber
                    });
            }
        }

        public async Task<IEnumerable<Reestr>> GetReestrsByBarrelNumbers(int from, int to)
        {
            using (var connection = _factory.CreateConnection())
            {
                string query = reestrSelectQuery + "where reestr.barrelNumber >=@from and reestr.barrelNumber <=@to";

                return await connection.QueryAsync<Reestr, Recipe, BarrelStorage, Customer, Reestr>(query,
                    (reestr, recipe, br, c) =>
                    {
                        reestr.Recipe = recipe;
                        reestr.BarrelStorage = br;
                        reestr.Customer = c;
                        return reestr;
                    }
                    , new
                    {
                        from ,
                        to 
                    });
            }
        }
    }
}
