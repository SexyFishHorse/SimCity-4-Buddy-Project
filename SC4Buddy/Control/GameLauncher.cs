namespace NIHEI.SC4Buddy.Control
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows.Forms;

    using NIHEI.SC4Buddy.Properties;

    public class GameLauncher
    {
        private readonly ProcessStartInfo gameProcessStartInfo;

        private readonly int autoSaveWaitTime;

        public GameLauncher(ProcessStartInfo gameProcessStartInfo, int autoSaveWaitTime)
        {
            this.gameProcessStartInfo = gameProcessStartInfo;
            this.autoSaveWaitTime = autoSaveWaitTime;
        }

        public void Start()
        {
            var gameProcess = Process.Start(gameProcessStartInfo);
            var handle = gameProcess.Handle;

            if (!Settings.Default.EnableAutoSave)
            {
                return;
            }

            while (true)
            {
                if (gameProcess.HasExited)
                {
                    break;
                }

                SetForegroundWindow(handle);
                SendKeys.SendWait("^%(s)");
                Thread.Sleep(autoSaveWaitTime * 60000);
            }
        }

        [DllImport("User32.dll")]
        private static extern int SetForegroundWindow(IntPtr handle);
    }
}
