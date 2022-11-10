using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Domain.Models;

namespace WeigthIndicator.Domain.Exceptions
{
    public class ReestrAlreadyOutcomedException : Exception
    {
        public ReestrAlreadyOutcomedException(Reestr reestr)
        {
            Reestr = reestr;
        }

        public Reestr Reestr { get; }
    }
}
