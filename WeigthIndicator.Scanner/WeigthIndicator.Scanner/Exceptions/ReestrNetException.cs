using System;
using System.Collections.Generic;
using System.Text;
using WeigthIndicator.Shared.Models;

namespace WeigthIndicator.Scanner.Exceptions
{
    public class ReestrNetException : Exception
    {
        public ReestrNetException(ReestrQrObject reestrQrObject)
        {
            ReestrQrObject = reestrQrObject;
        }

        public ReestrQrObject ReestrQrObject { get; }
    }
}
