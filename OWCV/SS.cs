using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace OWCV { 

    /// <summary>
    ///     This class shall keep the GDI32 APIs used in our program.
    /// </summary>
public class PlatformInvokeGdi32
{
    #region Class Variables

    public const int Srccopy = 13369376;

    #endregion

    #region Class Functions<br>

    [DllImport("gdi32.dll", EntryPoint = "DeleteDC")]
    public static extern IntPtr DeleteDC(IntPtr hDc);

    [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
    public static extern IntPtr DeleteObject(IntPtr hDc);

    [DllImport("gdi32.dll", EntryPoint = "BitBlt")]
    public static extern bool BitBlt(IntPtr hdcDest, int xDest,
        int yDest, int wDest, int hDest, IntPtr hdcSource,
        int xSrc, int ySrc, int rasterOp);

    [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleBitmap")]
    public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc,
        int nWidth, int nHeight);

    [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleDC")]
    public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

    [DllImport("gdi32.dll", EntryPoint = "SelectObject")]
    public static extern IntPtr SelectObject(IntPtr hdc, IntPtr bmp);

    #endregion
}

/// <summary>
///     This class shall keep the User32 APIs used in our program.
/// </summary>
public class PlatformInvokeUser32
{
    #region Class Variables

    public const int SmCxscreen = 0;
    public const int SmCyscreen = 1;

    #endregion

    #region Class Functions

    [DllImport("user32.dll", EntryPoint = "GetDesktopWindow")]
    public static extern IntPtr GetDesktopWindow();

    [DllImport("user32.dll", EntryPoint = "GetDC")]
    public static extern IntPtr GetDC(IntPtr ptr);

    [DllImport("user32.dll", EntryPoint = "GetSystemMetrics")]
    public static extern int GetSystemMetrics(int abc);

    [DllImport("user32.dll", EntryPoint = "GetWindowDC")]
    public static extern IntPtr GetWindowDC(int ptr);

    [DllImport("user32.dll", EntryPoint = "ReleaseDC")]
    public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDc);

    #endregion
}


/// This class shall keep all the functionality for capturing
/// the desktop.
public class CaptureScreen
{
    public static Bitmap CaptureApplication(string procName)
    {
        var proc = Process.GetProcessesByName(procName)[0];
        var rect = new User32.Rect();
        User32.GetWindowRect(proc.MainWindowHandle, ref rect);

        int width = rect.right - rect.left;
        int height = rect.bottom - rect.top;

        var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
        Graphics graphics = Graphics.FromImage(bmp);
        graphics.CopyFromScreen(rect.left, rect.top, 0, 0, new System.Drawing.Size(width, height), CopyPixelOperation.SourceCopy);

        return bmp;
    }

    private class User32
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);
    }
    #region Public Class Functions

    protected static IntPtr MHBitmap;

    public static Bitmap GetDesktopImage()
    {
        //In size variable we shall keep the size of the screen.
        Size size;

        //Variable to keep the handle to bitmap.
        IntPtr hBitmap;

        //Here we get the handle to the desktop device context.
        var hDc = PlatformInvokeUser32.GetDC
            (PlatformInvokeUser32.GetDesktopWindow());

        //Here we make a compatible device context in memory for screen
        //device context.
        var hMemDc = PlatformInvokeGdi32.CreateCompatibleDC(hDc);

        //We pass SM_CXSCREEN constant to GetSystemMetrics to get the
        //X coordinates of the screen.
        size.Cx = PlatformInvokeUser32.GetSystemMetrics
            (PlatformInvokeUser32.SmCxscreen);

        //We pass SM_CYSCREEN constant to GetSystemMetrics to get the
        //Y coordinates of the screen.
        size.Cy = PlatformInvokeUser32.GetSystemMetrics
            (PlatformInvokeUser32.SmCyscreen);

        //We create a compatible bitmap of the screen size and using
        //the screen device context.
        hBitmap = PlatformInvokeGdi32.CreateCompatibleBitmap
            (hDc, size.Cx, size.Cy);

        //As hBitmap is IntPtr, we cannot check it against null.
        //For this purpose, IntPtr.Zero is used.
        if (hBitmap != IntPtr.Zero)
        {
            //Here we select the compatible bitmap in the memeory device
            //context and keep the refrence to the old bitmap.
            var hOld = PlatformInvokeGdi32.SelectObject
                (hMemDc, hBitmap);
            //We copy the Bitmap to the memory device context.
            PlatformInvokeGdi32.BitBlt(hMemDc, 0, 0, size.Cx, size.Cy, hDc,
                0, 0, PlatformInvokeGdi32.Srccopy);
            //We select the old bitmap back to the memory device context.
            PlatformInvokeGdi32.SelectObject(hMemDc, hOld);
            //We delete the memory device context.
            PlatformInvokeGdi32.DeleteDC(hMemDc);
            //We release the screen device context.
            PlatformInvokeUser32.ReleaseDC(PlatformInvokeUser32.GetDesktopWindow(), hDc);
            //Image is created by Image bitmap handle and stored in
            //local variable.
            var bmp = Image.FromHbitmap(hBitmap);
            //Release the memory to avoid memory leaks.
            PlatformInvokeGdi32.DeleteObject(hBitmap);
            //This statement runs the garbage collector manually.
            GC.Collect();
            //Return the bitmap 
            return bmp;
        }
        //If hBitmap is null, retun null.
        return null;
    }

    #endregion
}

//This structure shall be used to keep the size of the screen.
public struct Size
{
    public int Cx;
    public int Cy;
}
}