using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;

namespace OWCV
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            var tick = new System.Timers.Timer(10);

            var gw = Utility.GetGameWindow();
            var outWindow = new Window("dst image");
            tick.Elapsed += (sender, eArgs) =>
            {
                using (var sc = ScreenCapture.CaptureWindow(gw))
                {
                    var source = OpenCvSharp.Extensions.BitmapConverter.ToMat(sc);
                    var FOV = 150; // px

                    var ROI = new Rect(960 - FOV, 540 - FOV, FOV * 2, FOV * 2);

                    var filtered = CV.FilterRed(source);
                    var thresholded = CV.Threshold(filtered);
                    var canny = CV.Canny(thresholded);
                    var contours = CV.Contours(canny, source);

                    outWindow.ShowImage(source);
                }
            };

            tick.Start();
        }
    }
}