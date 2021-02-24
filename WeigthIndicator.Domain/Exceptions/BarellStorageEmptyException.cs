using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeigthIndicator.Domain.Exceptions
{
    public class BarellStorageEmptyException : Exception
    {
        public BarellStorageEmptyException(string message):base(message)
        {
        }
        public BarellStorageEmptyException()
        {}
    }
}
