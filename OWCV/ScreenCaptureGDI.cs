﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using MaterialSkin;

namespace OWCV
{
    public class ScreenCaptureGDI
    {
        public IntPtr WindowHandle;
        public IntPtr hdcSrc;
        public IntPtr hdcDest;
        public IntPtr hBitmap;
        public DateTime LastFrame = DateTime.Now;

        public ScreenCaptureGDI(IntPtr windowHandle, int fov = 5)
        {
            WindowHandle = windowHandle;
            // TODO: optimize this to only copy over FOV
            // TODO: optimize this to reuse pointers and bitmaps
            // get te hDC of the target window
            hdcSrc = User32.GetWindowDC(windowHandle);

            // create a device context we can copy to
            hdcDest = GDI32.CreateCompatibleDC(hdcSrc);

            // create a bitmap we can copy it to,
            // using GetDeviceCaps to get the width/height
            hBitmap = GDI32.CreateCompatibleBitmap(hdcSrc, fov, fov);
        }

        /// <summary>
        /// Creates an Image object containing a screen shot of a specific window
        /// </summary>
        /// <param name="handle">The handle to the window. (In windows forms, this is obtained by the Handle property)</param>
        /// <param name="fov">Field of view</param>
        /// <returns></returns>
        public Bitmap CaptureWindow(int fov = 8)
        {
            // get the size
            User32.RECT windowRect = new User32.RECT();
            User32.GetWindowRect(WindowHandle, ref windowRect);
            int width = windowRect.right - windowRect.left;
            int height = windowRect.bottom - windowRect.top;

            // select the bitmap object
            IntPtr hOld = GDI32.SelectObject(hdcDest, hBitmap);


#if DEBUG
            // Draw FOV
            // CVUtils.DrawOnScreen(new Rectangle(width / 2 - fov / 2, height / 2 - fov / 2, fov, fov));
#endif

            // bitblt over
            GDI32.BitBlt(hdcDest, 0, 0, fov, fov, hdcSrc, width / 2 - fov / 2, height / 2 - fov / 2, GDI32.SRCCOPY);

            // restore selection
            GDI32.SelectObject(hdcDest, hOld);

            // get a .NET image object for it
            Bitmap img = Image.FromHbitmap(hBitmap);

            return img;
        }

        public Color[] GetCenterScreenPixels()
        {

            // get the size
            User32.RECT windowRect = new User32.RECT();
            User32.GetWindowRect(WindowHandle, ref windowRect);
            int width = windowRect.right - windowRect.left;
            int height = windowRect.bottom - windowRect.top;
#if DEBUG
            // Draw FOV
            // CVUtils.DrawOnScreen(new Rectangle(width / 2, height / 2, 1, 1));
#endif
            return new [] { GetPixel(width / 2, height / 2) };
        }

        public Color GetPixel(int x, int y)
        {
            uint pixel = GDI32.GetPixel(hdcSrc, x, y); //Returns ABGR

            var c = Color.FromArgb((int) pixel);
#if DEBUG
            //Debug.WriteLine("ms per frame: " + (DateTime.Now - LastFrame).TotalMilliseconds);
            //LastFrame = DateTime.Now;
#endif
            return Color.FromArgb(c.A, c.B, c.G, c.R);
        }

        /// <summary>
        /// Helper class containing Gdi32 API functions
        /// </summary>
        private class GDI32
        {
            [DllImport("gdi32.dll")]
            public static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);
            public const int SRCCOPY = 0x00CC0020; // BitBlt dwRop parameter
            [DllImport("gdi32.dll")]
            public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest,
                int nWidth, int nHeight, IntPtr hObjectSource,
                int nXSrc, int nYSrc, int dwRop);
            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth,
                int nHeight);
            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleDC(IntPtr hDC);
            [DllImport("gdi32.dll")]
            public static extern bool DeleteDC(IntPtr hDC);
            [DllImport("gdi32.dll")]
            public static extern bool DeleteObject(IntPtr hObject);
            [DllImport("gdi32.dll")]
            public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
        }

        /// <summary>
        /// Helper class containing User32 API functions
        /// </summary>
        private class User32
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                public int left;
                public int top;
                public int right;
                public int bottom;
            }
            [DllImport("user32.dll")]
            public static extern IntPtr GetDesktopWindow();
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowDC(IntPtr hWnd);
            [DllImport("user32.dll")]
            public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);
        }
    }
}