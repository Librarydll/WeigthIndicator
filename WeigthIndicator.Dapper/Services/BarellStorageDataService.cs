using Dapper;
using Dapper.Contrib.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeigthIndicator.Domain.Models;
using WeigthIndicator.Domain.Services;

namespace WeigthIndicator.Dapper.Services
{
    public class BarellStorageDataService : IBarellStorageDataService
    {
        private readonly ApplicationContextFactory _factory;

        public BarellStorageDataService(ApplicationContextFactory factory)
        {
            _factory = factory;
        }
        public async Task<BarellStorage> CreateBarellStorage(BarellStorage barellStorage)
        {
            using (var connection =_factory.CreateConnection())
            {
                await connection.InsertAsync(barellStorage);
                return barellStorage;
            }
        }

        public async Task<IEnumerable<BarellStorage>> GetBarellStorages()
        {
            using (var connection = _factory.CreateConnection())
            {
                string query = "SELECT *FROM BarellStorages as bs LEFT JOIN Recipes as r on bs.RecipeId = r.Id";

                var barellStorages = await connection.QueryAsync<BarellStorage, Recipe, BarellStorage>(query,
                    (rs, r) =>
                    {
                        rs.Recipe = r;
                        return rs;
                    });

                return barellStorages;

            }
        }
    }
}
