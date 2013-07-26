namespace NIHEI.SC4Buddy
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Windows.Forms;

    using log4net;
    using log4net.Config;

    using NIHEI.SC4Buddy.DataAccess;
    using NIHEI.SC4Buddy.Entities;
    using NIHEI.SC4Buddy.Localization;
    using NIHEI.SC4Buddy.Properties;
    using NIHEI.SC4Buddy.View.Application;

    public static class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        [STAThread]
        public static void Main(string[] args)
        {
            XmlConfigurator.Configure();

            Log.Info("Application starting");
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.ApplicationExit += (sender, eventArgs) => Log.Info("Application exited");
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en");

                if (string.IsNullOrWhiteSpace(Settings.Default.GameLocation))
                {
                    Application.Run(new SettingsForm { StartPosition = FormStartPosition.CenterScreen });
                    SetDefaultUserFolder();
                }

                if (!string.IsNullOrWhiteSpace(Settings.Default.GameLocation))
                {
                    Application.Run(new Sc4Buddy());
                }
            }
            catch (Exception ex)
            {
                Log.Error("Uncaught error", ex);
            }
        }

        private static void SetDefaultUserFolder()
        {
            var registry = RegistryFactory.UserFolderRegistry;

            var path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SimCity 4");

            if (!Directory.Exists(path) || registry.UserFolders.Any(x => x.Path.Equals(path)))
            {
                return;
            }

            Log.Info(string.Format("Setting default user folder to {0}", path));
            registry.Add(new UserFolder { Alias = LocalizationStrings.DefaultUserFolderName, Path = path });
        }
    }
}
