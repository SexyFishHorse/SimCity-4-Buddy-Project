namespace NIHEI.SC4Buddy
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Threading;
    using System.Windows.Forms;

    using NIHEI.SC4Buddy.Properties;
    using NIHEI.SC4Buddy.View.Application;

    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en");

            if (string.IsNullOrWhiteSpace(Settings.Default.GameLocation))
            {
                Application.Run(new SettingsForm { StartPosition = FormStartPosition.CenterScreen });
            }

            if (!string.IsNullOrWhiteSpace(Settings.Default.GameLocation))
            {
                Application.Run(new Sc4Buddy());
            }
        }
    }
}
