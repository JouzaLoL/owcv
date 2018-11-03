using Emgu.CV;
using Emgu.CV.Structure;
using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Drawing;
using Emgu.CV.CvEnum;
using FormTimer = System.Windows.Forms.Timer;
using ThreadTimer = System.Timers.Timer;

namespace OWCV
{
    public interface ILog
    {
        void Log(string message, Color? color = null);
    }

    public partial class MainForm : MaterialForm, ILog
    {
        public static ILog Form;

        public FormTimer FindOw = new FormTimer();
        public ThreadTimer TickTimer = new ThreadTimer();
        public IntPtr GameWindow;
        public int TickMsValue = 50;

        public MainForm()
        {
            InitializeComponent();

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900,
                Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);

#if DEBUG
            labelDebug.Visible = true;
#endif
            Form = this;

            FindOw.Tick += (s, a) =>
            {
                var gameWindow = Utility.GetGameWindow();
                if (gameWindow == IntPtr.Zero)
                {
                    Log($"Game instance not found. Retrying in {FindOw.Interval / 1000} seconds", Color.DarkRed);
                    FindOw.Stop();
                    FindOw.Interval += 1000;
                    FindOw.Start();
                }
                else
                {
                    Log("Game instance found! Injecting...", Color.DarkGreen);
                    FindOw.Stop();
                    GameWindow = gameWindow;
                    Inject(gameWindow);
                }
            };
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Log("Attempting to find game instance...");
            FindOw.Interval = 1;
            FindOw.Start();
        }

        /// <summary>
        /// Starts capturing OW
        /// </summary>
        public void Inject(IntPtr gameWindow)
        {
#if DEBUG
            CvInvoke.NamedWindow("Contours", NamedWindowType.FreeRatio);
#endif
            var gameWindowRes = ScreenCapture.GetWindowRes(gameWindow);

            // FOV
            var fov = new Size(200, 200);
            var roiRect =
                new Rectangle(
                    new Point(gameWindowRes.Width / 2 - fov.Height / 2, gameWindowRes.Height / 2 - fov.Height / 2),
                    fov);

            TickTimer = new ThreadTimer(TickMsValue);
            TickTimer.Elapsed += (s, a) => { ProcessFrame(fov, roiRect, gameWindow); };

            TickTimer.Start();
        }

        private void ProcessFrame(Size fov, Rectangle roiRect, IntPtr gameWindow)
        {
            try
            {
                var bmp = ScreenCaptureDesktop.CaptureWindow(gameWindow);
                var source = new Image<Bgr, byte>(bmp);
                bmp.Dispose();
#if DEBUG
                source.Draw(source.Data.Length.ToString(), new Point(40, 40), FontFace.HersheyDuplex, 1, new Bgr(Color.Red));
                source.Draw(roiRect, new Bgr(Color.AliceBlue));
#endif
                var roi = source.Copy(roiRect);
                CVMain.Pipeline(roi, fov);
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

        public void Log(string message)
        {
            richTextBox1.AppendText(message + "\n");
        }

        // ReSharper disable once MethodOverloadWithOptionalParameter
        public void Log(string message, Color? color = null)
        {
            richTextBox1.AppendText(message + "\n", color ?? Color.Black);
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void tickMs_Scroll(object sender, EventArgs e)
        {
            tickSpeedMsValue.Text = tickMs.Value.ToString();
        }

        private void materialRaisedButton1_Click(object sender, EventArgs e)
        {
            TickTimer.Stop();
            TickTimer.Interval = tickMs.Value;
            Log("Settings changed, reloading...", Color.DarkBlue);
            FindOw.Stop();
            Form1_Load(this, null);
        }


        private void labelDebug_Click(object sender, EventArgs e)
        {

        }
    }
}