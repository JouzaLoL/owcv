using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

// ReSharper disable InconsistentNaming

namespace OWCV
{
    public class InputHandler
    {
        private const uint WM_KEYDOWN = 0x0100;
        private const uint WM_KEYUP = 0x0101;
        private const int VK_K = 0x4B;
        private DateTime _lastFireTime;
        private readonly IntPtr _mainWindowHandle;

        public InputHandler(IntPtr mainWindowHandle)
        {
            _mainWindowHandle = mainWindowHandle;
            _lastFireTime = DateTime.Now;
        }

        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        private void PostMouseClick(int downTime = 100)
        {
            PostMessage(_mainWindowHandle, WM_KEYDOWN, VK_K, 0);
            Thread.Sleep(downTime);
            PostMessage(_mainWindowHandle, WM_KEYUP, VK_K, 0);
        }

        public void Fire(int fireDelay = 300)
        {
            var msSinceLastFire = DateTime.Now.Subtract(_lastFireTime).Milliseconds;
            if (msSinceLastFire > fireDelay)
            {
                PostMouseClick();
                _lastFireTime = DateTime.Now;
            }
        }
    }
}