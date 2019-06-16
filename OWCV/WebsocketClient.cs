using System;
using OWCV;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace OWCV
{
    public class Laputa : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            var msg = e.Data == "BALUS"
                ? "I've been balused already..."
                : "I'm not available now.";

            Send(msg);
        }
    }

    public class WebsocketClient
    {
        private WebSocketServer wsServer;
        public WebsocketClient()
        {
            wsServer = new WebSocketServer("ws://192.168.88.200:5555");
            wsServer.AddWebSocketService<Laputa>("/");
            wsServer.Start();
            wsServer.WebSocketServices.Broadcast("FIRE");
        }

        public void Fire()
        {
            wsServer.WebSocketServices.Broadcast("FIRE");
        }
    }
}
