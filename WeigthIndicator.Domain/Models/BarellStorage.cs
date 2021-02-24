using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Domain.Models.Common;

namespace WeigthIndicator.Domain.Models
{
    public class BarellStorage : BaseEntity,ICloneable
    {
        private DateTime _productionDate;

        public DateTime ProductionDate
        {
            get => _productionDate; 
            set => SetProperty( ref _productionDate , value); 
        }

        private double _totalWeight;

        public double TotalWeight
        {
            get => _totalWeight;
            set => SetProperty(ref _totalWeight, value);
        }

        private double _consumptionWeight;

        public double ConsumptionWeight
        {
            get => _consumptionWeight;
            set { SetProperty(ref _consumptionWeight, value); RaisePropertyChangedEvent("HasWeigthValue"); }
        }
        [Computed]
        public double HasWeigthValue => TotalWeight - ConsumptionWeight;

        public bool IsEmpty { get; set; }
        public int RecipeId { get; set; }
        [Computed]
        public Recipe Recipe { get; set; }

        public object Clone()
        {
            return new BarellStorage
            {
                Recipe = (Recipe)Recipe?.Clone(),
                ConsumptionWeight = ConsumptionWeight,
                Id = Id,
                IsEmpty = IsEmpty,
                ProductionDate = ProductionDate,
                RecipeId = RecipeId,
                TotalWeight = TotalWeight
            };
        }
    }
}
