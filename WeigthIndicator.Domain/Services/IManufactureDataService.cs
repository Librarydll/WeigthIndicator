using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Domain.Models;

namespace WeigthIndicator.Domain.Services
{
    public interface IManufactureDataService
    {
        Task<IEnumerable<Manufacture>> GetManufactures();

        Task<Manufacture> CreateManufacture(Manufacture manufacture);
        Task<bool> UpdateManufacture(Manufacture manufacture);
        Task<bool> DeleteManufacture(int id);
    }
}
