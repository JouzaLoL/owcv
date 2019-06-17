using System.Diagnostics;
using System.IO;

namespace OWCV
{
    public class InputProxy
    {
        private readonly StreamWriter _streamWriter;

        private const string FireCommand = @"printf '\x02\x00\x0e\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00' > /dev/hidg0";

        public InputProxy()
        {
            var sortProcess = new Process
            {
                StartInfo = { FileName = "./adb.exe", Arguments = "shell", UseShellExecute = false, RedirectStandardOutput = true }
            };


            // Redirect standard input as well.  This stream
            // is used synchronously.
            sortProcess.StartInfo.RedirectStandardInput = true;
            sortProcess.Start();

            // Use a stream writer to synchronously write the sort input.
            _streamWriter = sortProcess.StandardInput;

            _streamWriter.WriteLineAsync("su");
        }

        public enum Action : byte
        {
            Fire
        }

        public void SendAction(Action action)
        {
            Debug.WriteLine("Sending command to USB HID device");

            _streamWriter.WriteLineAsync(FireCommand);

        }
    }
}