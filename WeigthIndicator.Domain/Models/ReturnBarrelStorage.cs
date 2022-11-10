using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Domain.Models.Common;

namespace WeigthIndicator.Domain.Models
{
    public class ReturnBarrelStorage : BaseEntity
    {
        public User User { get; set; }
        public int UserId { get; set; }
        public DateTime Created { get; set; }
        public double ReturnCount { get; set; }
        public int BarrelStorageId { get; set; }
    }
}
