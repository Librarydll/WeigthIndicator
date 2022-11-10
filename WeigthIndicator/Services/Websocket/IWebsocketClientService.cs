using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp.Server;

namespace WeigthIndicator.Services
{
    public interface IWebsocketClientService
    {
        ConcurrentDictionary<string, WebSocketBehavior> Connections { get; }
        void AddClient(WebSocketBehavior behavior);
        void RemoveClient(string id);
    }
}
