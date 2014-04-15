namespace NIHEI.SC4Buddy.Control
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    using log4net;

    using NIHEI.SC4Buddy.Properties;

    using Timer = System.Threading.Timer;

    public class GameLauncher : IDisposable
    {
        private const int MillisecondsPrMinute = 60000;

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ProcessStartInfo gameProcessStartInfo;

        private readonly int autoSaveWaitTime;

        private Process gameProcess;

        private Timer timer;

        public GameLauncher(ProcessStartInfo gameProcessStartInfo, int autoSaveWaitTime)
        {
            this.gameProcessStartInfo = gameProcessStartInfo;
            this.autoSaveWaitTime = autoSaveWaitTime;
        }

        public void Start()
        {
            Log.Info(
                string.Format("Starting game with the following arguments: {0}", gameProcessStartInfo.Arguments));
            gameProcess = Process.Start(gameProcessStartInfo);
            gameProcess.Exited += (sender, args) => Dispose();
            var handle = gameProcess.Handle;

            if (!Settings.Default.EnableAutoSave)
            {
                return;
            }

            Log.Info(string.Format("Autosave is enabled. Attempting to save the game every {0} minutes.", autoSaveWaitTime));

            timer = new Timer(SendSaveCommand, handle, autoSaveWaitTime * MillisecondsPrMinute, autoSaveWaitTime * MillisecondsPrMinute);
        }

        public void Dispose()
        {
            timer.Dispose();
        }

        [DllImport("User32.dll")]
        private static extern int SetForegroundWindow(IntPtr handle);

        private void SendSaveCommand(object state)
        {
            Log.Info("Sending save signal to the game.");
            SetForegroundWindow((IntPtr)state);
            SendKeys.SendWait("^%(s)");
        }
    }
}
