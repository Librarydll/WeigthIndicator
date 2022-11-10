using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using WeigthIndicator.Shared.Models;

namespace WeigthIndicator.Shared.Services
{
    public class Encrtypter 
    {
       public static string Serialize(ReestrQrObject model)
       {
            return $"{model.Id};{model.BarrelNumber};{model.BatchNumber};{model.PackingDate};{model.ProductionDate};{model.Comment};{model.Net};{model.ProductionTitle}";
       }
        public static ReestrQrObject Deserialize(string query)
        {
            var splitted = query.Split(';');
            return new ReestrQrObject
            {
                Id = int.Parse(splitted[0]),
                BarrelNumber = int.Parse(splitted[1]),
                BatchNumber = splitted[2],
                PackingDate = DateTime.Parse(splitted[3]),
                ProductionDate = DateTime.Parse(splitted[4]),
                Comment = splitted[5],
                Net = double.Parse(splitted[6]),
                ProductionTitle = splitted[7]
            };
        }
    }
}
