using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace OWCV
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
#if DEBUG
            CvInvoke.NamedWindow("Contours", Emgu.CV.CvEnum.NamedWindowType.FreeRatio);            
#endif


            var gameWindow = Utility.GetGameWindow();

            // FOV
            var FOV = new Size(200, 200);
            var ROIRect =
                new Rectangle(new Point(Screen.PrimaryScreen.Bounds.Width/2 - FOV.Height / 2, Screen.PrimaryScreen.Bounds.Height / 2 - FOV.Height / 2),
                    FOV);

            var tick = new System.Timers.Timer(16);
            tick.Elapsed += (sender, eArgs) =>
            {
                try
                {
                    var bmp = ScreenCapture.CaptureWindow(gameWindow);
                    var source = new Image<Bgr, byte>(bmp);
                    bmp.Dispose();
#if DEBUG
                source.Draw(ROIRect, new Bgr(Color.AliceBlue));
#endif
                    var roi = source.Copy(ROIRect);
                    var filtered = CV.FilterRed(roi);
                    var thresholded = CV.Threshold(filtered);
                    var canny = CV.Canny(thresholded);
                    var contours = CV.Contours(canny, roi);
#if DEBUG
                CvInvoke.Imshow("Contours", source);                
#endif
                    roi.Dispose();
                    filtered.Dispose();
                    thresholded.Dispose();
                    canny.Dispose();
                    contours.Dispose();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            };

            tick.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}