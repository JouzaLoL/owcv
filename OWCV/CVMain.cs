using System;
using System.Diagnostics;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace OWCV
{
    public class CVMain
    {
        public enum ColorMode
        {
            Red,
            Magenta
        }

        public static Tuple<Hsv, Hsv> Red = new Tuple<Hsv, Hsv>(new Hsv(0, 82, 0), new Hsv(5, 255, 255));
        public static Tuple<Hsv, Hsv> Magenta = new Tuple<Hsv, Hsv>(new Hsv(132, 61, 170), new Hsv(163, 255, 255));

        public static bool Pipeline(Image<Bgr, byte> original, Size FOV, Tuple<Hsv, Hsv> colorRange)
        {
            var processed = original
                .Convert<Hsv, byte>()
                .InRange(new Hsv(132, 61, 170), new Hsv(163, 255, 255))
                .Dilate(1)
                .Erode(1)
                .SmoothGaussian(5)
                .ThresholdToZero(new Gray(50))
                .Canny(85, 10);

            var contours = new VectorOfVectorOfPoint();
            var hierarchy = new Mat();

            CvInvoke.FindContours(processed, contours, hierarchy, RetrType.External, ChainApproxMethod.ChainApproxNone);
            processed.Dispose();
            
            if (contours.Size == 0)
            {
                return false;
            }

#if !DEBUG
            original.Dispose();
#endif

#if DEBUG
            CvInvoke.DrawContours(original, contours, -1, new MCvScalar(100, 0, 0));
#endif
            var crosshair = new PointF(FOV.Height / 2, FOV.Height / 2 + 25f);
#if DEBUG
            original.Draw(new Rectangle((int)crosshair.X, (int)crosshair.Y, 1, 1), new Bgr(Color.Coral));
#endif

#if DEBUG
            CvInvoke.FillPoly(
                original,
                contours,
                new MCvScalar(255.0, 200, 0.0));
#endif

            for (var i = 0; i < contours.Size; i++)
            {
                var currObj = contours[i];
                var dist = CvInvoke.PointPolygonTest(currObj, crosshair, false);

                if (dist > -1)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool PipelineFast(Image<Bgr, byte> original, Size FOV, Tuple<Hsv, Hsv> colorRange)
        {
            var processed = original
                .Convert<Hsv, byte>()
                .InRange(colorRange.Item1, colorRange.Item2)
                .ThresholdToZero(new Gray(50));

            var crosshair = new Point(FOV.Height / 2, FOV.Height / 2);
#if DEBUG
            original.Draw(new Rectangle(crosshair.X, crosshair.Y, 1, 1), new Bgr(Color.Coral));
            CvInvoke.Imshow("Contours", processed);
#endif
            var mean = processed.GetAverage();
            
            if (mean.Intensity > 0)
            {
                return true;
            }
            return false;
        }

        public static Image<Gray, byte> FilterRed(Image<Bgr, byte> original)
        {
            var hsvImage = original.Convert<Hsv, byte>();
            return hsvImage.InRange(Red.Item1, Red.Item2);
        }

        public static Image<Gray, byte> Canny(Image<Gray, byte> original)
        {
            return original.Canny(255 / 3, 10);
        }

        public static Image<Gray, byte> Threshold(Image<Gray, byte> original)
        {
            var input = original.Dilate(1).Erode(1).SmoothGaussian(2);
            CvInvoke.Threshold(input, input, 50, 100, ThresholdType.ToZero);
            return input;
        }
    }
}