using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

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
                
            CvInvoke.FindContours(input, contours, hierarchy, RetrType.External, ChainApproxMethod.ChainApproxSimple);
            //CvInvoke.DrawContours(drawTarget, contours, -1, new MCvScalar(255, 0, 0));

            //for (var i = 0; i < contours.Size; i++)
            //{
            //    var line = new Mat();
            //    //CvInvoke.FitLine(contours[i], line, DistType.C, 0, 0.01, 0.01);

            //    //drawTarget.Draw(line.ToImage<Bgr, byte>(), new Bgr(0, 255, 0));
            //    var hull = new Mat();
            //    CvInvoke.ConvexHull(contours[i], hull);
            //    CvInvoke.Polylines(drawTarget, hull, false, new MCvScalar(0, 255, 0));
            //}

            ShouldFire(contours, drawTarget);

            return drawTarget;
        }

        public static bool ShouldFire(VectorOfVectorOfPoint contours, Image<Bgr, byte> drawTarget)
        {
            var centerOfScreen = Cursor.Position;
            Point pt = Cursor.Position; // Get the mouse cursor in screen coordinates 

            for (var i = 0; i < contours.Size; i++)
            {
                var hull = new Mat();
                var el = CvInvoke.BoundingRectangle(contours[i]);
                CvInvoke.ConvexHull(contours[i], hull);
                CvInvoke.Polylines(drawTarget, hull, false, new MCvScalar(0, 255, 0));
                drawTarget.Draw(el, new Bgr(Color.Aqua));
                var dist = CvInvoke.PointPolygonTest(hull, centerOfScreen, true);
                if (dist > 0)
                {
                    System.Media.SystemSounds.Exclamation.Play();
                    Utility.DoMouseClick();
                    return true;
                }
            }

            return false;
        }
    }
}