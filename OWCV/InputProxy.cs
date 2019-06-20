using System;
using System.IO.Ports;
using System.Threading;

namespace OWCV
{
    public class InputProxy
    {
        private SerialPort port;
        public InputProxy()
        {
            var ports = SerialPort.GetPortNames();
            port = new SerialPort(ports[1], 9600);
            port.Open();
        }

        public enum Action : byte
        {
            Fire
        }

        public void SendAction()
        {
            var data = new byte[1];
            data[0] = 0x0;
            port.Write(data, 0, 1);
        }
    }
}