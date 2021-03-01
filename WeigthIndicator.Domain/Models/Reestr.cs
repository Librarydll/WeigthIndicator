using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Domain.Models.Common;

namespace WeigthIndicator.Domain.Models
{
    public class Reestr : BaseEntity, ICloneable
    {
        private bool reestrState;
        private double net;
        private string note;
        private Customer customer;

        public int BarrelNumber { get; set; }
        public string BatchNumber { get; set; }
        public DateTime PackingDate { get; set; }
        /// <summary>
        /// 0-грузить,1-не грузить
        /// </summary>
        public bool ReestrState { get => reestrState; set => SetProperty(ref reestrState, value); }
        /// <summary>
        /// Чистый вес
        /// </summary>
        public double Net { get => net; set => SetProperty(ref net, value); }
        /// <summary>
        /// Вес самой бочки 
        /// </summary>
        public double TareBarrel { get; set; }
        /// <summary>
        /// Вес самой бочки с крышкой
        /// </summary>
        public double TareBarrelWithLid { get; set; }
        public string Note { get => note; set => SetProperty(ref note, value); }

        public int RecipeId { get; set; }
        public int BarrelStorageId { get; set; }
        public int CustomerId { get; set; }
        [Computed]
        public Recipe Recipe { get; set; }
        [Computed]
        public BarrelStorage BarrelStorage { get; set; }
        [Computed]
        public Customer Customer { get => customer; set => SetProperty(ref customer, value); }

        public object Clone()
        {
            return new Reestr
            {
                BarrelStorageId = BarrelStorageId,
                BarrelNumber = BarrelNumber,
                BatchNumber = BatchNumber,
                Id = Id,
                Net = Net,
                Note = Note,
                PackingDate = PackingDate,
                RecipeId = RecipeId,
                ReestrState = ReestrState,
                TareBarrel = TareBarrel,
                TareBarrelWithLid = TareBarrelWithLid,
                BarrelStorage = (BarrelStorage)BarrelStorage?.Clone(),
                Recipe = (Recipe)Recipe?.Clone(),
                Customer = (Customer)Customer?.Clone(),
                CustomerId = CustomerId
            };
        }
    }


}
