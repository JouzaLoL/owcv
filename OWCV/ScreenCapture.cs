using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace OWCV
{
    public static class ScreenCaptureDesktop
    {
        public static Bitmap CaptureDesktop()
        {
            var bp = new Bitmap(Screen.PrimaryScreen.Bounds.Size.Width, Screen.PrimaryScreen.Bounds.Size.Height);
            var g = Graphics.FromImage(bp);
            g.CopyFromScreen(0, 0, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);
            return bp;
        }

        public static Bitmap CaptureWindow(IntPtr windowHandle)
        {
            var windowRect = new ScreenCapture.User32.RECT();
            ScreenCapture.User32.GetClientRect(windowHandle, ref windowRect);

            var width = windowRect.right - windowRect.left;
            var height = windowRect.bottom - windowRect.top;

            var rect = new Rectangle(new Point(windowRect.left, windowRect.top), new Size(width, height));

            var bp = new Bitmap(width, height);
            using (var g = Graphics.FromImage(bp))
            {
                g.CopyFromScreen(windowRect.left, windowRect.top, 0, 0, new Size(width, height),
                    CopyPixelOperation.SourceCopy);
                return bp;
            }
        }
    }
}

public static class ScreenCapture
{
    /// <summary>
    ///     Creates an Image object containing a screen shot of a specific window
    /// </summary>
    /// <param name="handle">The handle to the window. (In windows forms, this is obtained by the Handle property)</param>
    /// <returns></returns>
    public static Bitmap CaptureWindow(IntPtr handle)
    {
        // get te hDC of the target window
        var desktop = User32.GetDesktopWindow();
        var hdcSrc = User32.GetWindowDC(handle);
        // get the size
        var windowRect = new User32.RECT();
        User32.GetWindowRect(handle, ref windowRect);
        var width = windowRect.right - windowRect.left;
        var height = windowRect.bottom - windowRect.top;
        // create a device context we can copy to
        var hdcDest = GDI32.CreateCompatibleDC(hdcSrc);
        // create a bitmap we can copy it to,
        // using GetDeviceCaps to get the width/height
        var hBitmap = GDI32.CreateCompatibleBitmap(hdcSrc, width, height);
        // select the bitmap object
        var hOld = GDI32.SelectObject(hdcDest, hBitmap);
        // bitblt over
        GDI32.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, GDI32.SRCCOPY);
        // restore selection
        GDI32.SelectObject(hdcDest, hOld);
        // clean up 
        GDI32.DeleteDC(hdcDest);
        User32.ReleaseDC(handle, hdcSrc);
        // get a .NET image object for it
        // free up the Bitmap object
        return Image.FromHbitmap(hBitmap);
    }

    public static Size GetWindowRes(IntPtr handle)
    {
        // get the size
        var windowRect = new User32.RECT();
        User32.GetWindowRect(handle, ref windowRect);
        var width = windowRect.right - windowRect.left;
        var height = windowRect.bottom - windowRect.top;

        return new Size(width, height);
    }

    /// <summary>
    ///     Helper class containing Gdi32 API functions
    /// </summary>
    public class GDI32
    {
        public const uint SRCCOPY = 13369376; // BitBlt dwRop parameter

        [DllImport("gdi32.dll")]
        public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest,
            int nWidth, int nHeight, IntPtr hObjectSource,
            int nXSrc, int nYSrc, uint dwRop);

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
    ///     Helper class containing User32 API functions
    /// </summary>
    public class User32
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);

        [DllImport("user32.dll")]
        public static extern bool GetClientRect(IntPtr hWnd, ref RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
    }
}