using System;
using System.Drawing;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;

namespace OWCV
{
    public static class CVUtils
    {
        public static bool IsScreenlocked(Image<Bgr, byte> frame)
        {
            //frame.AvgSdv(out var avg, new MCvScalar(10), frame.);
            //Image<Gray, Byte>[] channels = frame.Split();
            //Image<Gray, Byte> ImgHue = channels[0];
            //Image<Gray, Byte> ImgSat = channels[1];
            //Image<Gray, Byte> ImgVal = channels[2];

            //DenseHistogram histo1 = new DenseHistogram(255, new RangeF(0, 255));
        
            //histo1.Calculate(new[] { ImgVal }, true, null);

            //double[] minV;
            //double[] maxV;
            //Point[] minL;
            //Point[] maxL;


            //histo1.MinMax(out minV, out maxV, out minL, out maxL);

            //if (maxV < 10)
            //{
            //    return true;
            //}
            //return false;
            return false;
        }

        public static void DrawOnScreen(Rectangle rect)
        {
            IntPtr desktopPtr = ScreenCapture.User32.GetWindowDC(IntPtr.Zero);
            Graphics g = Graphics.FromHdc(desktopPtr);

            SolidBrush b = new SolidBrush(Color.Red);
            g.FillRectangle(b, rect);

            g.Dispose();
            ScreenCapture.User32.ReleaseDC(IntPtr.Zero, desktopPtr);
        }
    }
}