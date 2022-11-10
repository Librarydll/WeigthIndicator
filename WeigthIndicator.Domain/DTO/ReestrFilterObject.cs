using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Domain.Models;

namespace WeigthIndicator.Domain.DTO
{
    public class ReestrFilterObject
    {
        public IEnumerable<Recipe> Recipes { get; set; }
        public IEnumerable<string> BatchNumbers { get; set; }

    }
}
