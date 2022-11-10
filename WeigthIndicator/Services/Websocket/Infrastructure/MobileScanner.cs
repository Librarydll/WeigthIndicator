using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;
using WeigthIndicator.Domain.Services;

namespace WeigthIndicator.Services.Websocket
{
    public class MobileScanner : WebSocketBehavior
    {
        private readonly IWebsocketClientService _websocketClientService;
        private readonly IWebsocketMessageHandler _websocketMessageHandler;
        public MobileScanner(
            IWebsocketClientService websocketClientService,
            IWebsocketMessageHandler websocketMessageHandler)
        {
            _websocketClientService = websocketClientService;
            _websocketMessageHandler = websocketMessageHandler;
        }
        protected override void OnOpen()
        {
            _websocketClientService.AddClient(this);
            base.OnOpen();
        }
        protected override void OnClose(CloseEventArgs e)
        {
            _websocketClientService.RemoveClient(ID);

            base.OnClose(e);
        }
        protected override void OnError(ErrorEventArgs e)
        {
            _websocketClientService.RemoveClient(ID);
            base.OnError(e);
        }
        protected override void OnMessage(MessageEventArgs e)
        {
            if (e.IsText)
            {
                Task.Run(async () =>
                {
                    var backData = await _websocketMessageHandler.HandleMessage(e.Data);
                    if (!string.IsNullOrWhiteSpace(backData))
                    {
                        SendAsync(backData, isSuccess =>
                        {
                            Debug.WriteLine($"Messaged sended {isSuccess}");
                        });
                    }
                   
                });
              
            }
           
        }
    }
}
