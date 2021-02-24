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
    public class ReestrDataService : IReestrDataService
    {
        private readonly ApplicationContextFactory _applicationContextFactory;

        public ReestrDataService(ApplicationContextFactory applicationContextFactory)
        {
            _applicationContextFactory = applicationContextFactory;
        }


        public async Task<Reestr> CreateReestr(Reestr reestr)
        {
            using (var connection =_applicationContextFactory.CreateConnection())
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
    }
}
