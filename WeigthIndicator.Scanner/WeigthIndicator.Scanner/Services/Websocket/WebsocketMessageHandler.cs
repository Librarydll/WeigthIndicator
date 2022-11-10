using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Shared.Models;

namespace WeigthIndicator.Scanner.Services
{
    public class WebsocketMessageHandler : IWebsocketMessageHandler
    {
        private readonly IWebsocketClientService _websocketClientService;

        public event Action<AuthorizeResult> AuthorizationResult;
        public event Action<CreateOutcomeResult> CreateOutcomeResult;

        public event Action<bool> ConnectionChanged;
        public bool IsConnected => _websocketClientService.IsConnected;


        public WebsocketMessageHandler(
            IWebsocketClientService websocketClientService)
        {
            websocketClientService.MessageRecieved += WebscoketServer_MessageRecieved;
            websocketClientService.WebscoketClosed += WebscoketServer_WebscoketClosed;
            websocketClientService.WebsocketOpened += WebscoketServer_WebsocketOpened;
            _websocketClientService = websocketClientService;
        }

        private void WebscoketServer_WebsocketOpened()
        {
            ConnectionChanged?.Invoke(true);
        }

        private void WebscoketServer_WebscoketClosed()
        {
            ConnectionChanged?.Invoke(false);
        }

        private void WebscoketServer_MessageRecieved(string data)
        {
            var root = JsonConvert.DeserializeObject<WebsocketRootModel>(data);
            switch (root.MessageType)
            {
                case MessageType.AuthorizeResult:
                    HandleAuthorizeResult(root);
                    break;
                case MessageType.CreateOutcomeResult:
                    HandleCreateOutcomeResult(root);
                    break;
                default:
                    break;
            }
        }

        private void HandleCreateOutcomeResult(WebsocketRootModel root)
        {
            CreateOutcomeResult?.Invoke(root.CreateOutcomeResult);
        }

        private void HandleAuthorizeResult(WebsocketRootModel model)
        {
            AuthorizationResult?.Invoke(model.AuthorizeResult);
        }

        public void SendMessage(WebsocketRootModel model)
        {
            var json = JsonConvert.SerializeObject(model);
            _websocketClientService.Send(json);
        }
    }
}
