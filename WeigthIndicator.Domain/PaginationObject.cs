using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeigthIndicator.Domain
{
    public class PaginationObject<T>
    {
        public IEnumerable<T> Collection { get; set; }
        public int ItemsCount { get; set; }
    }
}
