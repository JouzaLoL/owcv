using WebSocketSharp;

namespace OWCV
{
    public class InputProxy
    {
        private readonly WebsocketServer _wsServer;

        public InputProxy()
        {
            _wsServer = new WebsocketServer();
        }

        public enum Action : byte
        {
            Fire
        }

        public void SendAction(Action action)
        {
            _wsServer.Send(new[] {(byte)action});
        }
    }
}