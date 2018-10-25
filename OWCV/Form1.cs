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
        System.Timers.Timer tick;
        Window outWindow;
        public Form1()
        {
            InitializeComponent();

            tick = new System.Timers.Timer(500);

            var gw = Utility.GetGameWindow();
            outWindow = new Window("dst image");
            tick.Elapsed += (sender, eArgs) =>
            {
                Tick();
            };

            tick.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Tick();
        }

        private void Tick()
        {
            using (var sc = ScreenCapture.CaptureDesktop())
            {
                var source = OpenCvSharp.Extensions.BitmapConverter.ToMat(sc);

                var FOV = 150; // px

                var ROI = new Rect(960 - FOV, 540 - FOV, FOV * 2, FOV * 2);

                var filtered = CV.FilterRed(source);
                outWindow.Image = source;
                var thresholded = CV.Threshold(filtered);
                //outWindow.Image = source;
                var canny = CV.Canny(thresholded);
                //outWindow.Image = source;
                var contours = CV.Contours(canny, source);
                //outWindow.Image = source;

            }
        }
    }
}