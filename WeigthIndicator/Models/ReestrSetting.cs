using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Domain.Models;

namespace WeigthIndicator.Models
{
    public class ReestrSetting : ICloneable
    {
        public Recipe CurrentRecipe { get; set; }
        public string BatchNumber { get; set; }
        public string BuyerName { get; set; }

        public object Clone()
        {
            var recipe = new Recipe()
            {
                Id = CurrentRecipe.Id
            };

            return new ReestrSetting
            {
                BatchNumber = BatchNumber,
                CurrentRecipe = recipe,
                BuyerName = BuyerName
            };
        }
    }
}
