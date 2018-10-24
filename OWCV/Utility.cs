using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OWCV
{
    internal class Utility
    {
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        private static extern IntPtr FindWindowByCaption(IntPtr zeroOnly, string lpWindowName);
        public static IntPtr GetGameWindow()
        {
            return FindWindowByCaption(IntPtr.Zero, "Overwatch");
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int AllocConsole();
    }
}
