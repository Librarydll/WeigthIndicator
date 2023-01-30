using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeigthIndicator.Models
{
    public class Manufacture
    {
        public string ManufactureName { get; set; }
        public string Index { get; set; }
        public string AddressRu { get; set; }
        public string AddressKz { get; set; }
    }

    public static class ManufactureProvider
    {
        public static string ManufactureType { get; set; }

        public static Manufacture GetManufacture() => ManufactureType == "Gazalkent" ? Gazalkent : Agromir;
        public static Manufacture GetManufacturePepsiCo() => ManufactureType == "Gazalkent" ? GazalkentPepsiCo : AgromirPepsiCo;

        private static Manufacture Gazalkent => new Manufacture
        {
            AddressKz = "Р.У., Ташкент облысы, Газалкент қаласы, А.Тимур к-сi,49",
            AddressRu = "Р.У., Ташкентская область, г.Газалкент ул А.Темура 49,",
            Index = "110700",
            ManufactureName = "ООО GAZALKENT MEVA",
        };
        private static Manufacture Agromir => new Manufacture
        {
            ManufactureName = "«AGROMIR» JV LLC/СП ООО «AGROMIR»",
            Index = "140300",
            AddressKz = "Өзбекстан Республикасы, Самарқанд облысы, Самарқанд ауданы, Гүлобод кенті",
            AddressRu = "Республика Узбекистан, Самаркандская область, Самаркандский район, п Гулобод"
        };
        public static Manufacture AgromirPepsiCo => new Manufacture
        {
            ManufactureName = "СП ООО \"AGROMIR\"",
            AddressRu = "140300 Республика Узбекистан, Самаркандская область, Самаркандский район, п Гулобод"
        };
        public static Manufacture GazalkentPepsiCo => new Manufacture
        {
            ManufactureName = "ООО \"Gazalkent Meva\"",
            AddressRu = "110700 Адрес.Республика Узбекистан Ташкентская область г.Газалкент ул А.Темура 49"
        };
    }

    public class ManufactureTitle
    {
        public ManufactureTitle(string title)
        {
            Title = title;
        }
        public string Title { get; set; }
    }
}
