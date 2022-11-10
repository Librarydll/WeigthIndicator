using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeigthIndicator.Dapper.Helper
{
    public static class EnumHelper
    {
        public static IEnumerable<T> GetUniqueFlags<T>(this T flags)
                    where T : Enum    // New constraint for C# 7.3
        {
            foreach (Enum value in Enum.GetValues(flags.GetType()))
                if (flags.HasFlag(value))
                    yield return (T)value;
        }
    }
}
