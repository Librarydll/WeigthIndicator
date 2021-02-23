using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Domain.Models.Common;

namespace WeigthIndicator.Domain.Models
{
    public class Order:BaseEntity
    {
        public int BarrelNumber { get; set; }
        public string Name { get; set; }
        public string BatchNumber { get; set; }
        public DateTime PackingDate { get; set; }
        public DateTime ProductionDate { get; set; }
        public OrderState OrderState { get; set; }
        public double Net { get; set; }
        public double Tare { get; set; }
        public string Note { get; set; }
    }

    public enum OrderState
    {
        Load,
        Unload
    }
}
