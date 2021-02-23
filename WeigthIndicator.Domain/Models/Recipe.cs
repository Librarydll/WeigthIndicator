using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Domain.Models.Common;

namespace WeigthIndicator.Domain.Models
{
    public class Recipe : BaseEntity
    {
        public string ShortName { get; set; }
        public string LongName { get; set; }
        public double Brix { get; set; }
        public string StorageCondition { get; set; }
        public string TransportationCondition { get; set; }

        public double Сarbohydrates { get; set; }
        public double VitaminC { get; set; }
        public double EnergyValue { get; set; }
        public double DryContent { get; set; }

    }
}
