using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Models;

namespace WeigthIndicator.Services
{
    public class ReestrSettingProvider:IReestrSettingProvider
    {
        public ReestrSetting ReestrSetting { get; set; }
        public ReestrSettingProvider()
        {
            ReestrSetting = new ReestrSetting();
            ReestrSetting.CurrentRecipe = new Domain.Models.Recipe();
        }
    }
}
