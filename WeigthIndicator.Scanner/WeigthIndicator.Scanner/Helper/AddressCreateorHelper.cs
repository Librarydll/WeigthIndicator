using System;
using System.Collections.Generic;
using System.Text;

namespace WeigthIndicator.Scanner.Helper
{
    public static class AddressCreateorHelper
    {
        public static string CreateWebsocketAddress(this string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress)) return string.Empty;

            return $"ws://{ipAddress}:7070/scanner";
        }
    }
}
