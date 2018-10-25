using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace OWCV
{
    public class CV
    {
        public static void Pipeline(Image<Bgr, byte> original, Size FOV)
        {
            var processed = original
                .Convert<Hsv, byte>()
                .InRange(new Hsv(0, 82, 0), new Hsv(5, 255, 255))
                .Dilate(1)
                .Erode(1)
                .SmoothGaussian(5)
                .ThresholdToZero(new Gray(50))
                .Canny(85, 10);

            var contours = new VectorOfVectorOfPoint();
            var hierarchy = new Mat();

            CvInvoke.FindContours(processed, contours, hierarchy, RetrType.External, ChainApproxMethod.ChainApproxNone);
#if DEBUG
            CvInvoke.DrawContours(original, contours, -1, new MCvScalar(100, 0, 0));
#endif
            processed.Dispose();

            var crosshair = new PointF(FOV.Height / 2 - 0.4f, FOV.Height / 2 + 11);
#if DEBUG
            original.Draw(new Rectangle((int)crosshair.X, (int)crosshair.Y, 1, 1), new Bgr(Color.Coral));
#endif
            for (var i = 0; i < contours.Size; i++)
            {
                var currObj = contours[i];
                var hull = CvInvoke.ConvexHull(currObj.ToArray().Select((p) => new PointF(p.X, p.Y)).ToArray());
                
                var dist = CvInvoke.PointPolygonTest(new VectorOfPointF(hull), crosshair, false);

                if (dist > -1)
                {
                    Utility.DoMouseClick();
                    return;
                }
            }
        }

        public static Image<Gray, byte> FilterRed(Image<Bgr, byte> original)
        {
            var hsvImage = original.Convert<Hsv, byte>();
            return hsvImage.InRange(new Hsv(0, 82, 0), new Hsv(5, 255, 255));
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

        public static Image<Bgr, byte> Contours(Image<Gray, byte> input, Image<Bgr, byte> drawTarget)
        {
            var contours = new VectorOfVectorOfPoint();
            var hierarchy = new Mat();

            CvInvoke.FindContours(input, contours, hierarchy, RetrType.External, ChainApproxMethod.ChainApproxNone);

            ShouldFire(contours, drawTarget);

            return drawTarget;
        }

        public static bool ShouldFire(VectorOfVectorOfPoint contours, Image<Bgr, byte> drawTarget)
        {
#if DEBUG
            drawTarget.Draw(new CircleF(new PointF(100, 100), 1), new Bgr(Color.Coral));
#endif

            for (var i = 0; i < contours.Size; i++)
            {
                var currObj = contours[i];
#if DEBUG
                drawTarget.DrawPolyline(currObj.ToArray(), true, new Bgr(Color.BlueViolet));
#endif
                var dist = CvInvoke.PointPolygonTest(currObj, new PointF(100, 100), false);

                if (dist > -1)
                {
                    System.Media.SystemSounds.Exclamation.Play();
                    Utility.DoMouseClick();
                    break;
                }
            }

            return false;
        }
    }
}