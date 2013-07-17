namespace NIHEI.SC4Buddy
{
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Security;
    using System.Threading;
    using System.Windows.Forms;

    using NIHEI.SC4Buddy.Control;
    using NIHEI.SC4Buddy.DataAccess;
    using NIHEI.SC4Buddy.Entities;
    using NIHEI.SC4Buddy.Localization;
    using NIHEI.SC4Buddy.Properties;
    using NIHEI.SC4Buddy.View.Application;

    public static class Program
    {
        private static readonly string LogSource = ConfigurationManager.AppSettings.Get("EventLogSource");

        [STAThread]
        public static void Main()
        {
            try
            {
                if (!EventLog.SourceExists(LogSource))
                {
                    EventLog.CreateEventSource(LogSource, ConfigurationManager.AppSettings.Get("EventLogName"));
                }
            }
            catch (SecurityException)
            {
                MessageBox.Show(
                    LocalizationStrings.TheFirstTimeYouRunThisProgramYouMustRunItAsAnAdministrator,
                    LocalizationStrings.AdditionalPermissionsRequired,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1);
                return;
            }

            EventLog.WriteEntry(LogSource, "SC4Buddy started.");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en");

            if (string.IsNullOrWhiteSpace(Settings.Default.GameLocation))
            {
                EventLog.WriteEntry(LogSource, "Game location not defined.");
                Application.Run(new SettingsForm { StartPosition = FormStartPosition.CenterScreen });
                SetDefaultUserFolder();
            }

            if (!string.IsNullOrWhiteSpace(Settings.Default.GameLocation))
            {
                SessionController.Instance.AttemptAutoLogin();
                Application.Run(new Sc4Buddy());
            }
        }

        private static void SetDefaultUserFolder()
        {
            var registry = RegistryFactory.UserFolderRegistry;

            var path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SimCity 4");

            if (Directory.Exists(path) && !registry.UserFolders.Any(x => x.Path.Equals(path)))
            {
                registry.Add(new UserFolder { Alias = LocalizationStrings.DefaultUserFolderName, Path = path });
                EventLog.WriteEntry(LogSource, "Default user folder is set.");
            }
            else
            {
                EventLog.WriteEntry(LogSource, "Unable to locate default user folder.");
            }
        }
    }
}
