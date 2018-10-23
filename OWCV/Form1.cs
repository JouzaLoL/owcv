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
            var tick = new System.Timers.Timer(100);

            tick.Elapsed += (sender, eArgs) =>
            {
                var bmp = CaptureScreen.GetDesktopImage();
                var img = new Image<Bgr, byte>(bmp);
                imageBox1.Image = img;

                var filtered = CV.FilterRed(img);
                imageBox2.Image = filtered;

                var canny = CV.Canny(filtered);
                imageBox3.Image = canny;
            };

            tick.Start();
        }

        private void imageBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
