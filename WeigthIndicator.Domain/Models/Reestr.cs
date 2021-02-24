using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Domain.Models.Common;

namespace WeigthIndicator.Domain.Models
{
    public class Reestr:BaseEntity
    {
        public int BarrelNumber { get; set; }
        public string BatchNumber { get; set; }
        public string Buyer { get; set; }
        public DateTime PackingDate { get; set; }
        public DateTime ProductionDate { get; set; }
        /// <summary>
        /// 0-грузить,1-не грузить
        /// </summary>
        public bool ReestrState { get; set; }
        /// <summary>
        /// Чистый вес
        /// </summary>
        public double Net { get; set; }
        /// <summary>
        /// Вес самой бочки 
        /// </summary>
        public double TareBarrel { get; set; }
        /// <summary>
        /// Вес самой бочки с крышкой
        /// </summary>
        public double TareBarrelWithLid { get; set; }
        public string Note { get; set; }

        public int RecipeId { get; set; }
        [Computed]
        public Recipe Recipe { get; set; }

    }

 
}
