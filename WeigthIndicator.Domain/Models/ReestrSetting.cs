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

        private string _buyerName;
        public string BuyerName
        {
            get { return _buyerName; }
            set { SetProperty(ref _buyerName, value); }
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

        public int RecipeId { get; set; }
        [Computed]
        public Recipe CurrentRecipe { get; set; }

        public object Clone()
        {
            var recipe = new Recipe()
            {
                Id = CurrentRecipe == null ? 0 : CurrentRecipe.Id,
                ShortName = CurrentRecipe == null ? "" : CurrentRecipe.ShortName
            };

            return new ReestrSetting
            {
                BatchNumber = BatchNumber,
                CurrentRecipe = recipe,
                BuyerName = BuyerName,
                TaraBarrel = TaraBarrel,
                Seconds = Seconds,
                TaraBarrelWithLid = TaraBarrelWithLid,
                MaxWeight =MaxWeight,
                MinWeight =MinWeight,
                Id =Id,
                RecipeId =RecipeId
            };
        }
    }
}
