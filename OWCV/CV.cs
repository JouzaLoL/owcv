using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Blob;
using Point = OpenCvSharp.Point;

namespace OWCV
{
    public class CV
    {
        public static Mat FilterRed(Mat original)
        {
            var output = new Mat();
            Cv2.InRange(original, new Scalar(0, 82, 0), new Scalar(5, 255, 255), original);
            return original;
        }

        public static Mat Canny(Mat original)
        {
            return original.Canny(255 / 3, 10);
        }

        public static Mat Threshold(Mat original)
        {
            //var input = original.Dilate(1).Erode(1).GaussianBlur(new OpenCvSharp.Size(2, 2), 0.01);
            var output = new Mat();
            Cv2.Threshold(original, output, 50, 100, ThresholdTypes.Binary);
            return output;
        }

        public static Mat Contours(Mat input, Mat drawTarget)
        {
            var hierarchy = new Mat();

            Cv2.FindContours(input, out var contours, hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

            //ShouldFire(contours, drawTarget);

            return drawTarget;
        }

        public static bool ShouldFire(Mat[] contours, Mat drawTarget)
        {
            Cv2.Rectangle(drawTarget, new Rect(Cursor.Position.X, Cursor.Position.Y, 300, 300), new Scalar(150, 100, 10));
            Cv2.DrawContours(drawTarget, contours, -1, Scalar.BlueViolet);

            foreach (var currObj in contours)
            {
                var dist = Cv2.PointPolygonTest(currObj, new Point2f(150, 150), true);

                Debug.WriteLine(dist);
                if (dist > -1)
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