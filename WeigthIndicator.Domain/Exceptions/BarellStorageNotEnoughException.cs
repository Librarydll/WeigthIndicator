using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeigthIndicator.Domain.Exceptions
{
    public class BarellStorageNotEnoughException : Exception
    {
        public double NeededWeight { get; }
        public double TotalWeight { get; set; }
        public BarellStorageNotEnoughException(string message):base(message)
        { }
        public BarellStorageNotEnoughException(double needed,double total, string message):base(message)
        {
            NeededWeight = needed;
            TotalWeight = total;
        }
    }
}
