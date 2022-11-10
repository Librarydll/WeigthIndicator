using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeigthIndicator.Services
{
    public interface IWebsocketServer : IDisposable
    {
        void Start();
    }
}
