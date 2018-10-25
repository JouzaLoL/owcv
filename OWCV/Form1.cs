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
using Emgu.CV;
using Emgu.CV.Structure;

namespace OWCV
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CvInvoke.NamedWindow("Contours", Emgu.CV.CvEnum.NamedWindowType.FreeRatio);

            var gameWindow = Utility.GetGameWindow();

            var tick = new System.Timers.Timer(100);
            tick.Elapsed += (sender, eArgs) =>
            {
                var bmp = ScreenCapture.CaptureWindow(gameWindow);
                var source = new Image<Bgr, byte>(bmp);
                imageBox1.Image = source;
                // FOV
                var FOV = 150; // px
                
                var filtered = CV.FilterRed(source);
                var thresholded = CV.Threshold(filtered);
                var canny = CV.Canny(thresholded);

                var contours = CV.Contours(canny, source);
                CvInvoke.Imshow("Contours", contours);
            };

            tick.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}