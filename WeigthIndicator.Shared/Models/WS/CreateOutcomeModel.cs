using System;
using System.Collections.Generic;
using System.Text;

namespace WeigthIndicator.Shared.Models
{
    public class CreateOutcomeModel
    {
        public IEnumerable<ReestrQrObject> Reestrs { get; set; }
        public string Comment { get; set; }
        public string Title { get; set; }
        public int UserId { get; set; }
    }
}
