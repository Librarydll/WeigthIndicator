using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Domain.DTO;

namespace WeigthIndicator.Domain.Services
{
    public interface IHelperDataService
    {
        Task<ReestrFilterObject> GetReestrFilterObject();
    }
}
