using System;
using System.Collections.Generic;
using System.Text;

namespace WeigthIndicator.Scanner.Services
{
    public interface IWebsocketClientService
    {
        bool IsConnected { get; }
        void Reconnect();
        void Connect();
        void Send(string data);
        event Action WebscoketClosed;
        event Action WebsocketOpened;
        event Action<string> MessageRecieved;

        void ChangeServerUrl(string newUrl);
    }
}
