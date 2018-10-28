using Emgu.CV;
using Emgu.CV.Structure;
using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Drawing;
using System.Threading.Tasks;
using FormTimer = System.Windows.Forms.Timer;
using ThreadTimer = System.Timers.Timer;

namespace OWCV
{
    public interface ILog
    {
        void Log(string message);
    }

    public partial class OWCV : MaterialForm, ILog
    {
        public FormTimer findOW = new FormTimer();
        public static ILog Form;
        public OWCV()
        {
            InitializeComponent();

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);

#if DEBUG
            labelDebug.Visible = true;
#endif
            Form = this;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Log("Attempting to find game instance...");
            findOW.Interval = 1000;
            findOW.Tick += (s, a) =>
            {
                var gameWindow = Utility.GetGameWindow();
                if (gameWindow == IntPtr.Zero)
                {
                    Log($"Game instance not found. Retrying in {findOW.Interval / 1000} seconds");
                    findOW.Stop();
                    findOW.Interval += 1000;
                    findOW.Start();
                }
                else
                {
                    Log("Game instance found! Injecting...");
                    Inject(gameWindow);
                }
            };

            findOW.Start();
        }

        /// <summary>
        /// Starts capturing OW
        /// </summary>
        public void Inject(IntPtr gameWindow)
        {
#if DEBUG
            CvInvoke.NamedWindow("Contours", Emgu.CV.CvEnum.NamedWindowType.FreeRatio);
#endif
            var gameWindowRes = ScreenCapture.GetWindowRes(gameWindow);

            // FOV
            var FOV = new Size(200, 200);
            var ROIRect =
                new Rectangle(new Point(gameWindowRes.Width / 2 - FOV.Height / 2, gameWindowRes.Height / 2 - FOV.Height / 2),
                    FOV);

            var tick = new ThreadTimer(50);
            tick.Elapsed += async (s, a) =>
            {
                await ProcessAsync(FOV, ROIRect, gameWindow);
            };

            tick.Start();
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

        private void materialRaisedButton1_Click(object sender, EventArgs e)
        {
            
        }

        private void labelDebug_Click(object sender, EventArgs e)
        {

        }

        public void Log(string message)
        {
            richTextBox1.AppendText(message + "\n");
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}