using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using WeigthIndicator.Domain.Models;

namespace WeigthIndicator.Models.ViewModels
{
    public class ReestrObject : ReactiveObject
    {
        public ReestrObject()
        {

        }
        public ReestrObject(Reestr reestr)
        {
            Id = reestr.Id;
            BarrelNumber = reestr.BarrelNumber;
            BatchNumber = reestr.BatchNumber;
            PackingDate = reestr.PackingDate;
            ReestrState = reestr.ReestrState;
            Net = reestr.Net;
            TareBarrel = reestr.TareBarrel;
            TareBarrelWithLid = reestr.TareBarrelWithLid;
            Note = reestr.Note;
            RecipeId = reestr.RecipeId;
            BarrelStorageId = reestr.BarrelStorageId;
            CustomerId = reestr.CustomerId;
            Recipe = (Recipe)reestr.Recipe?.Clone();
            BarrelStorage = (BarrelStorage)reestr.BarrelStorage?.Clone();
            Customer = (Customer)reestr.Customer?.Clone();
            Manufacture = reestr.Manufacture?.Clone();
            ManufactureId = reestr.ManufactureId;
        }

        public int Id { get; set; }
        public int BarrelNumber { get; set; }
        public string BatchNumber { get; set; }
        public DateTime PackingDate { get; set; }
        /// <summary>
        /// 0-грузить,1-не грузить
        /// </summary>
        [Reactive] public bool ReestrState { get; set; }
        /// <summary>
        /// Чистый вес
        /// </summary>
        [Reactive]  public double Net { get; set; }
        /// <summary>
        /// Вес самой бочки 
        /// </summary>
        public double TareBarrel { get; set; }
        /// <summary>
        /// Вес самой бочки с крышкой
        /// </summary>
        public double TareBarrelWithLid { get; set; }
        [Reactive] public string Note { get; set; }

        public int RecipeId { get; set; }
        public int BarrelStorageId { get; set; }
        public int CustomerId { get; set; }
        public int ManufactureId { get; set; }
        public Recipe Recipe { get; set; }
        public BarrelStorage BarrelStorage { get; set; }
        [Reactive] public Customer Customer { get; set; }
        [Reactive] public Manufacture Manufacture { get; set; }
        public double Brutto => Net + TareBarrel;
        public System.Drawing.Bitmap QrCode { get; set; }
        public Reestr BuildOut()
        {
            return new Reestr
            {
                BarrelNumber = BarrelNumber,
                Customer = Customer,
                BarrelStorageId = BarrelStorageId,
                BarrelStorage = BarrelStorage,
                BatchNumber = BatchNumber,
                CustomerId = CustomerId,
                Id = Id,
                Net = Net,
                Note = Note,
                PackingDate = PackingDate,
                Recipe = Recipe,
                RecipeId = RecipeId,
                ReestrState = ReestrState,
                TareBarrel = TareBarrelWithLid,
                TareBarrelWithLid = TareBarrelWithLid,
                Manufacture = Manufacture,
                ManufactureId = ManufactureId
            };
        }

    }
}
