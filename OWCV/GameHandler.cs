using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Timers;

namespace OWCV
{
    public class GameHandler
    {

        public Timer TickTimer = new Timer();
        public int TickMsValue = 8;

        public IntPtr GameWindow;
        public InputProxy InputProxy;

        public int FireDelay = 300;
        public DateTime LastFireTime;

        public GameHandler(IntPtr gameWindow)
        {
            GameWindow = gameWindow;
            InputProxy = new InputProxy();
        }

        /// <summary>
        /// Starts capturing OW
        /// </summary>
        public void Inject()
        {
#if DEBUG
            CvInvoke.NamedWindow("Contours", NamedWindowType.FreeRatio);
#endif
            // FOV
            var fov = new Size(5, 5);

            while (true)
            {
                ProcessFrame(fov, GameWindow);
            }

            TickTimer = new Timer(TickMsValue);
            TickTimer.Elapsed += (s, a) =>
            {
                ProcessFrame(fov, GameWindow);
            };

            TickTimer.Start();
        }

        private void ProcessFrame(Size fov, IntPtr gameWindow)
        {
            try
            {
                var bmp = ScreenCaptureGDI.CaptureWindow(gameWindow);
                var source = new Image<Bgr, byte>(bmp);

                bmp.Dispose();

                if (CVMain.PipelineFast(source, fov, CVMain.Magenta) && CanFire())
                {
                    Debug.WriteLine("Firing!");
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