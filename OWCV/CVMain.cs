using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Diagnostics;
using System.Drawing;

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

        public static Hsv ColorToHSV(Color color)
        {
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            var hue = color.GetHue();
            var saturation = (max == 0) ? 0 : (1d - (1d * min / max)) * 255;
            var value = max;

            return new Hsv(hue, saturation, value);
        }

        public static bool PipelineSuperFastHsv(Color[] colors)
        {
            var hsv = ColorToHSV(colors[0]);
            Debug.WriteLine(hsv.ToString());
            return CompareHSV(hsv, Magenta.Item1, Magenta.Item2);
        }

        public static bool CompareHSV(Hsv input, Hsv lower, Hsv upper)
        {

            return (input.Hue >= lower.Hue && input.Hue <= upper.Hue &&
                    input.Satuation >= lower.Satuation && input.Satuation <= upper.Satuation &&
                    input.Value >= lower.Value && input.Value <= upper.Value);
        }

        public static bool PipelineSuperFast(Color[] colors)
        {
            if (IsRed(colors[0]))
            {
                return true;
            }

            return false;
        }

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

        public static bool PipelineFast(Image<Bgr, byte> original, Tuple<Hsv, Hsv> colorRange)
        {
            var processed = original
                .Convert<Hsv, byte>()
                .InRange(colorRange.Item1, colorRange.Item2);

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

        public static bool IsRed(Color color)
        {
            int minR = 150;//175
            int maxR = 255;//255
            int minG = 0;//0
            int maxG = 75;//75
            int minB = 0;//0
            int maxB = 90;//50
            if (color.R >= minR && color.R <= maxR &&
                color.G >= minG && color.G <= maxG &&
                color.B >= minB && color.B <= maxB)
            {
                return true;
            }

            return false;
        }
    }
}