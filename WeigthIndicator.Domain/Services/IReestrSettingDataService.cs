using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Domain.Models;

namespace WeigthIndicator.Domain.Services
{
    public interface IReestrSettingDataService
    {
        Task<ReestrSetting> CreateReestrSetting(ReestrSetting reestr);
        Task<bool> UpdateReestrSetting(ReestrSetting reestr);

        Task<ReestrSetting> GetReestrSetting();
    }
}
