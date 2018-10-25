using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Drawing;
using System.Windows.Forms;

namespace OWCV
{
    public class CV
    {
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