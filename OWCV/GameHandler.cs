using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Timers;
using System.Windows.Forms;

namespace OWCV
{
    public class GameHandler
    {
        public IntPtr GameWindow;
        public InputProxy InputProxy;
        public InputHandler InputHandler;

        public int FireDelay = 300;
        public DateTime LastFireTime;

        public ScreenCaptureGDI ScreenCapture;

        public GameHandler(IntPtr gameWindow)
        {
            GameWindow = gameWindow;
            InputProxy = new InputProxy();
            InputHandler = new InputHandler(gameWindow);
            ScreenCapture = new ScreenCaptureGDI(gameWindow);
        }

        /// <summary>
        /// Starts capturing OW
        /// </summary>
        public void Inject()
        {
            var s = new Stopwatch();
            while (true)
            {
                s.Reset();
                s.Start();
                ProcessFrame();
                // Debug.WriteLine(s.ElapsedMilliseconds);
            }
        }

        private void ProcessFrame()
        {
            try
            {
                var bmp = ScreenCapture.CaptureWindow();
                var source = new Image<Bgr, byte>(bmp);
                bmp.Dispose();

                if (CVMain.PipelineFast(source, CVMain.Magenta) && CanFire())
                {
                    InputProxy.SendAction(InputProxy.Action.Fire);
                }

                source.Dispose();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                throw;
            }
        }

        private bool CanFire()
        {
            if ((DateTime.Now - LastFireTime).TotalMilliseconds > FireDelay)
            {
                LastFireTime = DateTime.Now;
                return true;
            }
            return false;
        }
    }
}