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

            CvInvoke.NamedWindow("Contours", Emgu.CV.CvEnum.NamedWindowType.Normal);
            CvInvoke.NamedWindow("Canny", Emgu.CV.CvEnum.NamedWindowType.Normal);
            CvInvoke.NamedWindow("Filtered", Emgu.CV.CvEnum.NamedWindowType.Normal);

            var tick = new System.Timers.Timer(100);
            tick.Elapsed += (sender, eArgs) =>
            {
                var bmp = ScreenCapture.CaptureWindow(Utility.GetGameWindow());
                var img = new Image<Bgr, byte>(bmp);

                var filtered = CV.FilterRed(img);
                CvInvoke.Imshow("Filtered", filtered);

                var canny = CV.Canny(filtered);
                CvInvoke.Imshow("Canny", canny);

                var contours = CV.Contours(canny, img);
                CvInvoke.Imshow("Contours", contours);
            };

            tick.Start();
        }

        private void imageBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
