using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Domain.Models;

namespace WeigthIndicator.Domain.Services
{
    public interface IBarellStorageDataService
    {
        Task<BarellStorage> CreateBarellStorage(BarellStorage barellStorage);
        Task<IEnumerable<BarellStorage>> GetBarellStorages();
    }
}
