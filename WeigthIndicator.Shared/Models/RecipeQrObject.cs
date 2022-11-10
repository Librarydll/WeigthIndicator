using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace WeigthIndicator.Shared.Models
{
    public class RecipeQrObject
    {
        public int Id { get; set; }
        public string ShortName { get; set; }
        public string LongNameRu { get; set; }
        public string LongNameKz { get; set; }
        public double Brix { get; set; }
        public string StorageCondition { get; set; }
        public string TransportationCondition { get; set; }

        public double Carbohydrates { get; set; }
        public double VitaminC { get; set; }
        public double EnergyValue { get; set; }
        public double DryContent { get; set; }

        public BarrelRecipeTypeQr BarrelRecipeType { get; set; }

    }

    public enum BarrelRecipeTypeQr
    {
        Mettalic,
        Plastic
    }
}
