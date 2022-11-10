using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;
using WeigthIndicator.Domain.Services;
using WeigthIndicator.Services.Websocket;

namespace WeigthIndicator.Services
{
    public class WebsocketServer : IWebsocketServer ,IDisposable
    {
        private readonly string _url;
        private readonly IWebsocketClientService _websocketClientService;
        private readonly IWebsocketMessageHandler _websocketMessageHandler;
        private WebSocketServer _webSocket;

        public WebsocketServer(string url,
            IWebsocketClientService websocketClientService,
            IWebsocketMessageHandler websocketMessageHandler)
        {
            _url = url;
            _websocketClientService = websocketClientService;
            _websocketMessageHandler = websocketMessageHandler;
        }

        public void Start()
        {
            _webSocket = new WebSocketServer(_url);
            //_webSocket.
            _webSocket.AddWebSocketService("/scanner", () =>
            {
                return new MobileScanner(_websocketClientService, _websocketMessageHandler);
            });

            _webSocket.Start();  
        }
        
        public void Dispose()
        {
            _webSocket?.Stop();
        }

    }
}
