using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeigthIndicator.Domain.Models
{
    public interface IQrGenerator<T>
    {
         T GenerateQRObject();
    }
}
