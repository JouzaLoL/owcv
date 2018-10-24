using System;
using System.Collections.Generic;
using Emgu.CV;
using Emgu.CV.Structure;

namespace OWCV
{
    public class CV
    {
        public static Image<Gray, byte> FilterRed(Image<Bgr, byte> original)
        {
            var red = new Rgb(215, 54, 2);
            var hsvImage = original.Convert<Hsv, byte>();
            var lowerLimit = new Hsv(0, 100, 0);
            var upperLimit = new Hsv(5, 255, 255);
            return hsvImage.InRange(lowerLimit, upperLimit);
        }

        public static Image<Gray, byte> Canny(Image<Gray, byte> original)
        {
            return original.Canny(1, 100);
        }

        public static Image<Rgb, byte> Contours(Image<Gray, byte> input, Image<Bgr, byte> drawTarget)
        { 
            var imgout = drawTarget.Convert<Rgb, byte>();

            try
            {
                var contours = new Emgu.CV.Util.VectorOfVectorOfPoint();
                var hierarchy = new Mat();
                CvInvoke.FindContours(input, contours, hierarchy, Emgu.CV.CvEnum.RetrType.List, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);
                CvInvoke.DrawContours(imgout, contours, -1, new MCvScalar(255, 0, 0));

                for (int i = 0; i < contours.Size; i++)
                {
                    var bbox = CvInvoke.BoundingRectangle(contours[i]);
                    imgout.Draw(bbox, new Rgb(255, 0, 0));
                }
            }
            catch (Exception)
            {
                
                throw;
            }
            return imgout.Convert<Rgb, byte>();
        }
    }
}