using Dapper;
using Dapper.Contrib.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeigthIndicator.Domain.Models;
using WeigthIndicator.Domain.Services;

namespace WeigthIndicator.Dapper.Services
{
    public class BarrelStorageDataService : IBarrelStorageDataService
    {
        private readonly ApplicationContextFactory _factory;

        public BarrelStorageDataService(ApplicationContextFactory factory)
        {
            _factory = factory;
        }
        public async Task<BarrelStorage> CreateBarrelStorage(BarrelStorage barrelStorage)
        {
            using (var connection =_factory.CreateConnection())
            {
                await connection.InsertAsync(barrelStorage);
                return barrelStorage;
            }
        }

        public async Task<IEnumerable<BarrelStorage>> GetBarrelStorages()
        {
            using (var connection = _factory.CreateConnection())
            {
                string query = "SELECT *FROM barrelStorages as bs LEFT JOIN Recipes as r on bs.RecipeId = r.Id";

                var barrelStorages = await connection.QueryAsync<BarrelStorage, Recipe, BarrelStorage>(query,
                    (rs, r) =>
                    {
                        rs.Recipe = r;
                        return rs;
                    });

                return barrelStorages;

            }
        }

        public async Task<double> GetBarrelStorageRemainderByRecipe(int recipeId)
        {
            using (var connection = _factory.CreateConnection())
            {
                string query = "SELECT SUM(b.TotalWeight-B.ConsumptionWeight) FROM barrelstorages as b where IsEmpty = false and recipeid =@recipeId ";

                var reminder =await connection.QueryFirstOrDefaultAsync<double?>(query,new { recipeId });
                return reminder ?? 0;
            }
        }
    }
}
