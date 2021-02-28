using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Domain.Models;

namespace WeigthIndicator.Models
{
    public class PrintModel
    {
        public Recipe Recipe { get; set; }
        public Reestr Reestr { get; set; }

        public PrintModel(Recipe recipe,Reestr reestr)
        {
            Recipe = recipe;
            Reestr = reestr;
        }
    }
}
