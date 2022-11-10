using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Domain.DTO;
using WeigthIndicator.Domain.Models;
using WeigthIndicator.Domain.Services;

namespace WeigthIndicator.Dapper.Services
{
    public class HelperDataService : IHelperDataService
    {
        private readonly ApplicationContextFactory _factory;

        public HelperDataService(ApplicationContextFactory factory)
        {
            _factory = factory;
        }

        public async Task<ReestrFilterObject> GetReestrFilterObject()
        {
            using (var connection = _factory.CreateConnection())
            {
                var recipes = await connection.GetAllAsync<Recipe>();
                var distinctQuery = @"SELECT distinct(BatchNumber) FROM indicatordb.reestrs
                                     order by batchnumber desc";
                var batches = await connection.QueryAsync<string>(distinctQuery);

                return new ReestrFilterObject
                {
                    BatchNumbers = batches,
                    Recipes = recipes
                };
            }
        }
    }
}
