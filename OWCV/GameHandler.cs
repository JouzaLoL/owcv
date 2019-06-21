using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace OWCV
{
    public class GameHandler
    {
        public IntPtr GameWindow;
        public InputProxy InputProxy;
        public InputHandler InputHandler;

        public int FireDelay = 150;
        public DateTime LastFireTime;

        public ScreenCaptureGDI ScreenCapture;

        public DateTime EventSentTime = DateTime.Now;

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
            while (true)
            {
                Thread.Sleep(1);
                ProcessFrame();
            }

        }

        private void ProcessFrame()
        {
            //  Triggerbot only - compare pixel value
            //if (CVMain.PipelineSuperFastHsv(ScreenCapture.GetCenterScreenPixels()) && CanFire())
            //{
            //    EventSentTime = DateTime.Now;
            //    InputProxy.SendAction();
            //    // InputHandler.Fire();
            //}

            try
            {
                var bmp = ScreenCapture.CaptureWindow(50);
                var source = new Image<Bgr, byte>(bmp);
                bmp.Dispose();

                if (CVMain.PipelineFast(source, CVMain.Red) && CanFire() && GetAsyncKeyState(Keys.ShiftKey) < 0)
                {
                    InputProxy.SendAction();
                }

                source.Dispose();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                throw;
            }
        }

        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(Keys vKey);

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