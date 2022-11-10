using System;
using System.Collections.Generic;
using System.Text;

namespace WeigthIndicator.Shared.Models
{
    public class ReestrQrObject 
    {
        public int Id { get; set; }
        public int BarrelNumber { get; set; }
        public string BatchNumber { get; set; }
        public DateTime PackingDate { get; set; }
        public DateTime ProductionDate { get; set; }
        /// <summary>
        /// Чистый вес
        /// </summary>
        public double Net { get; set; }
        public string Comment { get; set; }
        public string ProductionTitle { get; set; }
    }
}
