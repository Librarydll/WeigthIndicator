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
    public class CustomerDataService : ICustomerDataService
    {
        private ApplicationContextFactory _factory;

        public CustomerDataService(ApplicationContextFactory factory)
        {
            _factory = factory;
        }

        public async Task<Customer> CreateCustomer(Customer customer)
        {
            using (var connection = _factory.CreateConnection())
            {
                await connection.InsertAsync(customer);
                return customer;
            }
        }

        public async Task<IEnumerable<Customer>> GetCustomers()
        {
            using (var connection = _factory.CreateConnection())
            {
                 return  await connection.GetAllAsync<Customer>();
            }
        }
    }
}
