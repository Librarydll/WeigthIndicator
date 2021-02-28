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
        Task<Reestr> CreateReestrAndUpdateBarellStorage(Reestr recipe);
        Task<bool> UpdateReestr(Reestr reestr);

        Task<IEnumerable<Reestr>> GetReestrsByDate(DateTime dateTime);

    }
}
