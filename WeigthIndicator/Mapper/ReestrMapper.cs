using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Domain.Models;
using WeigthIndicator.Models.ViewModels;
using WeigthIndicator.Shared.Models;

namespace WeigthIndicator.Mapper
{
    public static class ReestrMapper
    {
        public static ReestrQrObject Map(this ReestrObject reestr)
        {
            return new ReestrQrObject
            {
                BarrelNumber = reestr.BarrelNumber,
                Id = reestr.Id,
                BatchNumber = reestr.BatchNumber,
                Comment = reestr.Note,
                Net = reestr.Net,
                PackingDate = reestr.PackingDate,
                ProductionDate = reestr.BarrelStorage.ProductionDate,
                ProductionTitle = reestr.Recipe.LongNameRu
                //Recipe = new RecipeQrObject
                //{
                //    BarrelRecipeType = (BarrelRecipeTypeQr)(int)reestr.Recipe.BarrelRecipeType,
                //    Brix = reestr.Recipe.Brix,
                //    Carbohydrates = reestr.Recipe.Carbohydrates,
                //    DryContent = reestr.Recipe.DryContent,
                //    EnergyValue = reestr.Recipe.EnergyValue,
                //    Id = reestr.Id,
                //    LongNameKz = reestr.Recipe.LongNameKz,
                //    LongNameRu = reestr.Recipe.LongNameRu,
                //    ShortName = reestr.Recipe.ShortName,
                //    StorageCondition = reestr.Recipe.StorageCondition,
                //    TransportationCondition = reestr.Recipe.TransportationCondition,
                //    VitaminC = reestr.Recipe.VitaminC
                //}
            };
        }
    }
}
