using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Drawing;
using FormTimer = System.Windows.Forms.Timer;
using ThreadTimer = System.Timers.Timer;

namespace OWCV
{
    public interface ILog
    {
        void Log(string message, Color? color = null);
        void UpdatePicture(Bitmap b);
    }

    public partial class MainForm : MaterialForm, ILog
    {
        public static ILog Form;

        public FormTimer FindOw = new FormTimer();
        public int FindOwRetryTime = 500;

        public Color ErrorColor = Color.DarkRed;
        public Color InfoColor = Color.DarkBlue;
        public Color SuccessColor = Color.DarkGreen;

        public GameHandler GameHandler;

        public MainForm()
        {
            InitializeComponent();

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900,
                Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);

#if DEBUG
            Log("Running in debug mode.", InfoColor);
#endif

            Form = this;

            FindOw.Tick += (s, a) =>
            {
                var gameWindow = Utility.GetGameWindow();
                if (gameWindow == IntPtr.Zero)
                {
                    Log($"Game instance not found. Retrying in {FindOw.Interval / 1000} seconds", ErrorColor);
                    FindOw.Stop();
                    FindOw.Interval += FindOwRetryTime;
                    FindOw.Start();
                }
                else
                {
                    if (Utility.GetForegroundWindow() != gameWindow)
                    {
                        Log("Game instance found but not in focus. Bringing to focus and injecting...", SuccessColor);
                        Utility.BringToFront(gameWindow);
                        FindOw.Stop();
                        GameHandler = new GameHandler(gameWindow);
                        GameHandler.Inject();
                        return;
                    }

                    Log("Game instance found! Injecting...", SuccessColor);
                    FindOw.Stop();
                    GameHandler = new GameHandler(gameWindow);
                    GameHandler.Inject();
                }
            };
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Log("Attempting to find game instance...");
            FindOw.Interval = 1;
            FindOw.Start();
        }

        

        public void Log(string message)
        {
            richTextBox1.AppendText(message + "\n");
        }

        public void UpdatePicture(Bitmap bp)
        {
            pictureBox1.Image = bp;
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
            Log("Settings changed, reloading...", Color.DarkBlue);
            FindOw.Stop();
            Form1_Load(this, null);
        }


        private void labelDebug_Click(object sender, EventArgs e)
        {

        }

        private void materialFlatButton1_Click(object sender, EventArgs e)
        {
        }
    }
}