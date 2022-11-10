using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Domain.Models;
using WeigthIndicator.Domain.Services;

namespace WeigthIndicator.Dapper.Services
{
    public class ManufactureDataService : IManufactureDataService
    {
        private ApplicationContextFactory _factory;

        public ManufactureDataService(ApplicationContextFactory factory)
        {
            _factory = factory;
        }


        public async Task<Manufacture> CreateManufacture(Manufacture manufacture)
        {
            using (var connection = _factory.CreateConnection())
            {
                manufacture.Id =  await connection.InsertAsync(manufacture);
                return manufacture;
            }
        }

        public async Task<bool> DeleteManufacture(int id)
        {
            using (var connection = _factory.CreateConnection())
            {
                string query = "Delte from Manufactures where id =@id";
                var rowReturned = await connection.ExecuteAsync(query, new { id });
                return rowReturned > 0;
            }
        }

        public async Task<IEnumerable<Manufacture>> GetManufactures()
        {
            using (var connection = _factory.CreateConnection())
            {
                return await connection.GetAllAsync<Manufacture>();
            }

        }

        public async Task<bool> UpdateManufacture(Manufacture manufacture)
        {
            using (var connection = _factory.CreateConnection())
            {
                await connection.UpdateAsync(manufacture);

                return true;
            }
        }
    }
}
