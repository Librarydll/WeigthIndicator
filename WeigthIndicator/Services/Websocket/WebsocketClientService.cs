using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp.Server;

namespace WeigthIndicator.Services
{
    public class WebsocketClientService : IWebsocketClientService
    {
        public ConcurrentDictionary<string, WebSocketBehavior> Connections { get; } = new ConcurrentDictionary<string, WebSocketBehavior>();

        public void AddClient(WebSocketBehavior behavior)
        {
            Connections.TryAdd(behavior.ID, behavior);
        }

        public void RemoveClient(string id)
        {
            Connections.TryRemove(id, out WebSocketBehavior behavior);
        }
    }
}
