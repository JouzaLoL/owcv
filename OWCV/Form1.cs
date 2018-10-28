using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OWCV
{
    public partial class Form1 : Form
    {
        public void Inject()
        {
#if DEBUG
            CvInvoke.NamedWindow("Contours", Emgu.CV.CvEnum.NamedWindowType.FreeRatio);            
#endif

            var gameWindow = Utility.GetGameWindow();
            var gameWindowRes = ScreenCapture.GetWindowRes(gameWindow);
            // FOV
            var FOV = new Size(200, 200);
            var ROIRect =
                new Rectangle(new Point(gameWindowRes.Width / 2 - FOV.Height / 2, gameWindowRes.Height / 2 - FOV.Height / 2),
                    FOV);

            var tick = new System.Timers.Timer(50);
            tick.Elapsed += async (s, a) =>
            {
                await ProcessAsync(FOV, ROIRect, gameWindow);
            };

            tick.Start();
        }

        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private async Task ProcessAsync(Size FOV, Rectangle ROIRect, IntPtr gameWindow)
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
                CV.Pipeline(roi, FOV);
#if DEBUG
                CvInvoke.Imshow("Contours", roi);                
#endif
                source.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void btnInject_Click(object sender, EventArgs e)
        {
            Inject();
        }
    }
}