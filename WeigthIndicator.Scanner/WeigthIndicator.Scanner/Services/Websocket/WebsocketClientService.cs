using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using WebSocketSharp;

namespace WeigthIndicator.Scanner.Services
{
    public class WebsocketClientService : IWebsocketClientService
    {
        private string _url;
        private WebSocket _webSocket;

        public event Action WebscoketClosed;
        public event Action WebsocketOpened;
        public event Action<string> MessageRecieved;
        public bool IsConnected
        {
            get
            {
                if (_webSocket == null) return false;
                return _webSocket.IsAlive;
            }
        }
        public WebsocketClientService(string url)
        {
            _url = url;
        }

        public void Connect()
        {
            if (string.IsNullOrWhiteSpace(_url)) return;
            if (IsConnected) return;


            _webSocket = new WebSocket(_url);

            _webSocket.OnOpen += WebSocketOnOpen;
            _webSocket.OnMessage += WebSocketOnMessage;
            _webSocket.OnError += WebSocketOnError;
            _webSocket.OnClose += WebSocketOnClose;

            _webSocket.ConnectAsync();
        }
        public void Send(string data)
        {
            _webSocket.SendAsync(data, (isSuccess) =>
            {
                Debug.WriteLine($"Stauts :{isSuccess} - {data}");
            });
        }
        private void WebSocketOnClose(object sender, CloseEventArgs e)
        {
            WebscoketClosed?.Invoke();
        }

        private void WebSocketOnError(object sender, ErrorEventArgs e)
        {
            Debug.WriteLine($"Error websocket {e}");
            WebscoketClosed?.Invoke();
        }

        private void WebSocketOnMessage(object sender, MessageEventArgs e)
        {
            if (!e.IsPing)
            {
                MessageRecieved?.Invoke(e.Data);
            }
        }

        private void WebSocketOnOpen(object sender, EventArgs e)
        {
            WebsocketOpened?.Invoke();
        }

        public void Dispose()
        {
            _webSocket?.Close();
        }

        public void ChangeServerUrl(string newUrl)
        {
            _url = newUrl;
        }
        public void Reconnect()
        {
            _webSocket?.CloseAsync(CloseStatusCode.Normal);
            Connect();
        }
    }
}
