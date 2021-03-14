using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Domain.Models;

namespace WeigthIndicator.Domain.Services
{
    public interface IReestrDataService
    {
        Task<Reestr> CreateReestr(Reestr reestr);
        Task<Reestr> CreateReestrAndUpdateBarrelStorage(Reestr recipe);
        Task<bool> UpdateReestr(Reestr reestr);
        Task<IEnumerable<Reestr>> GetReestrsByDate(DateTime dateTim);

        Task<IEnumerable<Reestr>> GetReestrsByDate(DateTime dateTim,int typee);
        Task<IEnumerable<Reestr>> GetReestrsByDates(DateTime fromDate,DateTime toDate, int type);

        Task<IEnumerable<Reestr>> GetReestrsByBatchNumber(string batchNumber,int type);
        Task<IEnumerable<Reestr>> GetReestrsByBarrelNumbers(int from,int to, int type);

    }
}
