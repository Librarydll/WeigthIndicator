using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeigthIndicator.Domain.Exceptions
{
    public class BarrelStorageNotEnoughException : Exception
    {
        public double NeededWeight { get; }
        public double TotalWeight { get; set; }
        public BarrelStorageNotEnoughException(string message):base(message)
        { }
        public BarrelStorageNotEnoughException(double needed,double total, string message):base(message)
        {
            NeededWeight = needed;
            TotalWeight = total;
        }
    }
}
