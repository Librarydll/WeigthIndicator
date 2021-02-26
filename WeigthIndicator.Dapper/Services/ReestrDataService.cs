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

        public async Task<Reestr> CreateReestrAndUpdateBarellStorage(Reestr reestr)
        {
            using (var connection = _factory.CreateConnection())
            {
                bool isNextBarellExist = true;
                string barellQuery = "SELECT *FROM BarellStorages where isEmpty = false and recipeId =@recipeId";

                var barellStorage = await connection.QueryFirstOrDefaultAsync<BarellStorage>(barellQuery, new { recipeId = reestr.RecipeId });

                if (barellStorage == null)
                {
                    throw new BarellStorageEmptyException();
                }

                if (barellStorage.ConsumptionWeight + reestr.Net > barellStorage.TotalWeight)
                {

                    barellQuery += " and id != @id";

                    var nextBarell = await connection.QueryFirstOrDefaultAsync<BarellStorage>(barellQuery, new { recipeId = reestr.RecipeId, id = barellStorage.Id });
                    if (nextBarell != null)
                    {
                        var reminder = barellStorage.TotalWeight - barellStorage.ConsumptionWeight;
                        nextBarell.TotalWeight += reminder;
                        barellStorage.TotalWeight -= reminder;

                        string emptyUpdateQuery = "UPDATE BarellStorages Set isEmpty,totalWeight =@TotalWeight = true WHERE id =@id";
                        await connection.QueryAsync(emptyUpdateQuery, new { id = barellStorage.Id, totalWeight = barellStorage.TotalWeight });
                        barellStorage = nextBarell;
                    }
                    else
                    {
                        isNextBarellExist = false;
                    }


                }
                if(isNextBarellExist)
                    barellStorage.ConsumptionWeight += reestr.Net;


                await connection.UpdateAsync(barellStorage);

                reestr.BarellStorageId = barellStorage.Id;
                reestr.BarellStorage = barellStorage;


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
    }
}
