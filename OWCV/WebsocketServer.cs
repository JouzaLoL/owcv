using System;
using System.Diagnostics;
using OWCV;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace OWCV
{
    public class Laputa : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {

        }
    }

    public class WebsocketServer
    {
        private WebSocketServer wsServer;
        public WebsocketServer()
        {
            Utility.StartADBPortForward();
            wsServer = new WebSocketServer("ws://192.168.88.200:5555");
            wsServer.AddWebSocketService<Laputa>("/");
            wsServer.Start();
        }

        public void Send(byte[] command)
        {
            wsServer.WebSocketServices.Broadcast(command);
        }
    }
}
