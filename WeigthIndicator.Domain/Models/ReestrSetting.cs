using Dapper.Contrib.Extensions;
using System;
using WeigthIndicator.Domain.Models.Common;

namespace WeigthIndicator.Domain.Models
{
    public class ReestrSetting : BaseEntity, ICloneable
    {
        private string _batchNumber;
        public string BatchNumber
        {
            get { return _batchNumber; }
            set { SetProperty(ref _batchNumber, value); }
        }

        private double _taraBarrel;
        public double TaraBarrel
        {
            get { return _taraBarrel; }
            set { SetProperty(ref _taraBarrel, value); }

        }

        private double _taraBarrelWithLid;
        public double TaraBarrelWithLid
        {
            get { return _taraBarrelWithLid; }
            set { SetProperty(ref _taraBarrelWithLid, value); }

        }
        private int _seconds;
        public int Seconds
        {
            get { return _seconds; }
            set { SetProperty(ref _seconds, value); }
        }

        private int _initialBarrelNumber;

        public int InitialBarrelNumber
        {
            get => _initialBarrelNumber;
            set => SetProperty(ref _initialBarrelNumber, value);
        }

        private double _minWeight;
        public double MinWeight
        {
            get { return _minWeight; }
            set { SetProperty(ref _minWeight, value); }
        }

        private double _maxWeight;
        public double MaxWeight
        {
            get { return _maxWeight; }
            set { SetProperty(ref _maxWeight, value); }
        }

        private double _minDefaultWeigth;
        public double MinDefaultWeigth
        {
            get { return _minDefaultWeigth; }
            set { SetProperty(ref _minDefaultWeigth, value); }
        }
        public int ManufactureId { get; set; }
        public int CustomerId { get; set; }
        public int RecipeId { get; set; }
        [Computed]
        public Recipe CurrentRecipe { get; set; }
        [Computed]
        public Customer Customer { get; set; }

        [Computed]
        public Manufacture Manufacture { get; set; }

        public object Clone()
        {

            return new ReestrSetting
            {

                BatchNumber = BatchNumber,
                CurrentRecipe = (Recipe)CurrentRecipe?.Clone(),
                TaraBarrel = TaraBarrel,
                Seconds = Seconds,
                TaraBarrelWithLid = TaraBarrelWithLid,
                MaxWeight = MaxWeight,
                MinWeight = MinWeight,
                MinDefaultWeigth = MinDefaultWeigth,
                Id = Id,
                RecipeId = RecipeId,
                CustomerId = CustomerId,
                Customer = (Customer)Customer?.Clone(),
                InitialBarrelNumber = InitialBarrelNumber,
                Manufacture = Manufacture?.Clone(),
                ManufactureId = ManufactureId
            };
        }

        public int GetBarrelNumberAndClearIt()
        {
            if (InitialBarrelNumber != 0)
            {
                var initial = InitialBarrelNumber;
                InitialBarrelNumber = 0;
                return initial;
            }
            return InitialBarrelNumber;
        }
    }
}
