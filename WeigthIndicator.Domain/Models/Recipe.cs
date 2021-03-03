using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Domain.Models.Common;

namespace WeigthIndicator.Domain.Models
{
    public class Recipe : BaseEntity, ICloneable
    {
        private string shortName;
        private string longNameRu;
        private string longNameKz;
        private double brix;
        private string storageCondition;
        private string transportationCondition;
        private double сarbohydrates;
        private double vitaminC;
        private double energyValue;
        private double dryContent;

        public string ShortName { get => shortName; set => SetProperty(ref shortName, value); }
        public string LongNameRu { get => longNameRu; set => SetProperty(ref longNameRu, value); }
        public string LongNameKz { get => longNameKz; set => SetProperty(ref longNameKz, value); }
        public double Brix { get => brix; set => SetProperty(ref brix, value); }
        public string StorageCondition { get => storageCondition; set => SetProperty(ref storageCondition, value); }
        public string TransportationCondition { get => transportationCondition; set => SetProperty(ref transportationCondition, value); }

        public double Carbohydrates { get => сarbohydrates; set => SetProperty(ref сarbohydrates, value); }
        public double VitaminC { get => vitaminC; set => SetProperty(ref vitaminC, value); }
        public double EnergyValue { get => energyValue; set => SetProperty(ref energyValue, value); }
        public double DryContent { get => dryContent; set => SetProperty(ref dryContent, value); }

        public object Clone()
        {
            return new Recipe
            {
                Brix = Brix,
                DryContent = DryContent,
                EnergyValue = EnergyValue,
                Id = Id,
                LongNameRu = LongNameRu,
                ShortName = ShortName,
                StorageCondition = StorageCondition,
                TransportationCondition = TransportationCondition,
                VitaminC = VitaminC,
                Carbohydrates = Carbohydrates,
                LongNameKz = LongNameKz
            };
        }
    }
}
