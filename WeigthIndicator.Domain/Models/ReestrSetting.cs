using Dapper.Contrib.Extensions;
using System;
using WeigthIndicator.Domain.Models.Common;

namespace WeigthIndicator.Domain.Models
{
    public class ReestrSetting : BaseEntity, ICloneable
    {

        public string BatchNumber { get; set; }
        public string BuyerName { get; set; }
        public double TaraBarrel { get; set; }
        public double TaraBarrelWithLid { get; set; }
        public int Seconds { get; set; }
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
                TaraBarrelWithLid = TaraBarrelWithLid
            };
        }
    }
}
