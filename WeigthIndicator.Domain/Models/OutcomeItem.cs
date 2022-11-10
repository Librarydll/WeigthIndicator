using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Domain.Models.Common;

namespace WeigthIndicator.Domain.Models
{
    public class OutcomeItem : BaseEntity
    {
        /// <summary>
        /// Чистый вес который был отправлен
        /// </summary>
        public double Count { get; set; }
        public string Comment { get; set; }
        public int ReestrId { get; set; }
        public Reestr Reestr { get; set; }
        public int OutcomeId { get; set; }
        public Outcome Outcome { get; set; }
    }
}
