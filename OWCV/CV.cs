using System;
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
    }
}