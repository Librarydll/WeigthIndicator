using System;
using System.Collections.Generic;
using System.Text;
using WeigthIndicator.Shared.Models;

namespace WeigthIndicator.Scanner.Services
{
    public interface IWebsocketMessageHandler
    {
        bool IsConnected { get; }
        event Action<AuthorizeResult> AuthorizationResult;
        event Action<CreateOutcomeResult> CreateOutcomeResult;
        event Action<bool> ConnectionChanged;
        void SendMessage(WebsocketRootModel model);
    }
}
