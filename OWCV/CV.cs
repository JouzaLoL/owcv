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
            var input = original.Dilate(1).Erode(1).SmoothGaussian(5);
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
            var centerOfScreen = Cursor.Position;
            drawTarget.Draw(new CircleF(new PointF(centerOfScreen.X, centerOfScreen.Y), 10), new Bgr(Color.Coral));

            for (var i = 0; i < contours.Size; i++)
            {
                var currObj = contours[i];
                drawTarget.DrawPolyline(currObj.ToArray(), true, new Bgr(Color.BlueViolet));
                var dist = CvInvoke.PointPolygonTest(currObj, new PointF(centerOfScreen.X, centerOfScreen.Y), true);

                Debug.WriteLine(dist);
                if (dist > -1)
                {
                    Debug.WriteLine(dist);
                    System.Media.SystemSounds.Exclamation.Play();
                    Utility.DoMouseClick();
                    return true;
                }
            }

            return false;
        }
    }
}