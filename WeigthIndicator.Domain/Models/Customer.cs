using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Domain.Models.Common;

namespace WeigthIndicator.Domain.Models
{
    public class Customer : BaseEntity, ICloneable
    {
        private string _shortNmae;
        public string ShortName
        {
            get { return _shortNmae; }
            set { SetProperty(ref _shortNmae, value); }
        }
  
        public string AddressRu { get; set; }
        public string AddressKz { get; set; }

        public object Clone()
        {
            return new Customer
            {
                Id = Id,
                AddressRu = AddressRu,
                AddressKz = AddressKz,
                ShortName = ShortName
            };
        }
    }
}
