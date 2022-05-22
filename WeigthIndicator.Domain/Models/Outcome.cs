using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Domain.Models.Common;

namespace WeigthIndicator.Domain.Models
{
    public class Outcome : BaseEntity
    {
        public string Title { get; set; }
        public string Comment { get; set; }
        public DateTime Created { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

        public ICollection<OutcomeItem> OutcomeItems { get; set; }
    }
}
