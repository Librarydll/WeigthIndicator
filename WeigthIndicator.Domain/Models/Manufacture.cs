using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Domain.Models.Common;

namespace WeigthIndicator.Domain.Models
{
    public class Manufacture : BaseEntity
    {
        public string Name { get; set; }
        public string AddressRu { get; set; }
        public string AddressKz { get; set; }
        public string Index { get; set; }

        public Manufacture Clone()
        {
            return new Manufacture
            {
                AddressKz = AddressKz,
                AddressRu = AddressRu,
                Id = Id,
                Index = Index,
                Name = Name
            };
        }
    }
}
