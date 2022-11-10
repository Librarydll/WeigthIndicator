using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Shared.Models;

namespace WeigthIndicator.Services
{
    public interface IWebsocketMessageHandler
    {
        event Action<ReestrQrObject> ReestrAddedEvent;
        event Action<ReestrQrObject> ReestrRemovedEvent;
        Task<string> HandleMessage(string data);
    }
}
