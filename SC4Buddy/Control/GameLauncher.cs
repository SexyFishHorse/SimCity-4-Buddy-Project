namespace NIHEI.SC4Buddy.Control
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    using NIHEI.SC4Buddy.Properties;

    using Timer = System.Threading.Timer;

    public class GameLauncher : IDisposable
    {
        private const int MillisecondsPrMinute = 60000;

        private readonly ProcessStartInfo gameProcessStartInfo;

        private readonly int autoSaveWaitTime;

        private Process gameProcess;

        private Timer timer;

        public GameLauncher(ProcessStartInfo gameProcessStartInfo, int autoSaveWaitTime)
        {
            this.gameProcessStartInfo = gameProcessStartInfo;
            this.autoSaveWaitTime = autoSaveWaitTime;
            Running = true;
        }

        protected bool Running { get; set; }

        public void Start()
        {
            gameProcess = Process.Start(gameProcessStartInfo);
            gameProcess.Exited += (sender, args) => Dispose();
            var handle = gameProcess.Handle;

            if (!Settings.Default.EnableAutoSave)
            {
                return;
            }

            timer = new Timer(SendSaveCommand, handle, autoSaveWaitTime * MillisecondsPrMinute, autoSaveWaitTime * MillisecondsPrMinute);
        }

        public void Dispose()
        {
            Running = false;
            timer.Dispose();
        }

        [DllImport("User32.dll")]
        private static extern int SetForegroundWindow(IntPtr handle);

        private void SendSaveCommand(object state)
        {
            SetForegroundWindow((IntPtr)state);
            SendKeys.SendWait("^%(s)");
        }
    }
}
