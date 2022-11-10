using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WeigthIndicator.Scanner.Exceptions;
using WeigthIndicator.Shared.Models;
using WeigthIndicator.Shared.Services;

namespace WeigthIndicator.Scanner.Store
{
    public class ReestrsStore
    {
        public int ReestrsCount => _reestrs.Count;

        public double TotalNet => _reestrs.Sum(x => x.Net);
        public string Comment { get; set; }
        public string Title { get; set; }

        private HashSet<ReestrQrObject> _reestrs;
        public IEnumerable<ReestrQrObject> Reestrs => _reestrs;
        public ReestrsStore()
        {
            _reestrs = new HashSet<ReestrQrObject>();
        }

        public bool RemoveReestr(ReestrQrObject reestrQrObject)
        {
            return _reestrs.Remove(reestrQrObject);
        }
      
        public bool TryAddReestr(string json,out ReestrQrObject reestrQrObject)
        {
            reestrQrObject =null;
            try
            {
                reestrQrObject = Encrtypter.Deserialize(json);
            }          
            catch (Exception)
            {
                return false;
            }
            AddReestr(reestrQrObject);
            return true;
        }
        private ReestrQrObject AddReestr(ReestrQrObject reestrQrObject)
        {
            var existed = _reestrs.FirstOrDefault(x => x.BarrelNumber == reestrQrObject.BarrelNumber &&
            x.BatchNumber == reestrQrObject.BatchNumber &&
            x.PackingDate == reestrQrObject.PackingDate &&
            x.ProductionDate == reestrQrObject.ProductionDate);


            if (reestrQrObject.Net <= 0)
            {
                throw new ReestrNetException(reestrQrObject);
            }

            if (existed != null)
            {
                throw new ReestrAlreadyExistException();
            }

            _reestrs.Add(reestrQrObject);

            return reestrQrObject;
        }
        public void Reset()
        {
            _reestrs.Clear();
            Comment = string.Empty;
            Title = string.Empty;
        }
    }
}
