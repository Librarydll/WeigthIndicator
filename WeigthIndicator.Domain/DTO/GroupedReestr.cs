using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeigthIndicator.Domain.DTO
{
    public class GroupedReestr
    {
        public string BatchNumber { get; set; }
        public string RecipeName { get; set; }
        public int BarrelsCount { get; set; }
        public double TotalNet { get; set; }
    }
}
