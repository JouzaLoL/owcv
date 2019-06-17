using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;

namespace OWCV
{
    internal class Utility
    {
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        
        public static void BringToFront(IntPtr pointer)
        {
            SetForegroundWindow(pointer);
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        public static IntPtr GetGameWindow()
        {
            var gameWindow = Process.GetProcessesByName("Overwatch");
            if (gameWindow.Length == 0)
            {
                return IntPtr.Zero;
            }
            else
            {
                return gameWindow[0].MainWindowHandle;
            }   
        }

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        private static extern IntPtr FindWindowByCaption(IntPtr zeroOnly, string lpWindowName);
        public static IntPtr GetGameWindow2()
        {
            return FindWindowByCaption(IntPtr.Zero, "Overwatch");
        }
    }

    public static class RichTextBoxExtensions
    {
        public static void AppendText(this RichTextBox box, string text, Color color)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;
            box.SelectionFont = new Font(Control.DefaultFont, FontStyle.Bold);
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
        }
    }
}
