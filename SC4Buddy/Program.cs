namespace NIHEI.SC4Buddy
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Forms;

    using log4net;
    using log4net.Config;

    using NIHEI.SC4Buddy.Control;
    using NIHEI.SC4Buddy.Control.Plugins;
    using NIHEI.SC4Buddy.Control.UserFolders;
    using NIHEI.SC4Buddy.DataAccess;
    using NIHEI.SC4Buddy.Localization;
    using NIHEI.SC4Buddy.Model;
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
                MoveDatabaseIfExists();

                var userFolderController = new UserFolderController(EntityFactory.Instance.Entities);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.ApplicationExit += (sender, eventArgs) => Log.Info("Application exited");

                if (string.IsNullOrWhiteSpace(Settings.Default.GameLocation) || !Directory.Exists(Settings.Default.GameLocation))
                {
                    var settingsForm = new SettingsForm(userFolderController) { StartPosition = FormStartPosition.CenterScreen };

                    Application.Run(settingsForm);
                    SetDefaultUserFolder();
                }

                if (userFolderController.UserFolders.Any(x => x.IsMainFolder && x.FolderPath.Equals("?")))
                {
                    SetDefaultUserFolder();
                }

                if (Directory.Exists(Settings.Default.GameLocation))
                {
                    new SettingsController(userFolderController).UpdateMainFolder();
                    Application.Run(
                        new Sc4Buddy(
                            userFolderController,
                            new PluginController(EntityFactory.Instance.Entities),
                            new PluginGroupController(EntityFactory.Instance.Entities)));
                }
            }
            catch (Exception ex)
            {
                Log.Error("Uncaught error", ex);

                var showLog = MessageBox.Show(
                    string.Format(LocalizationStrings.UncaughtExceptionWouldYouLikeToOpenTheLog, ex.Message),
                    LocalizationStrings.UncaughtException,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1);

                if (showLog == DialogResult.No)
                {
                    return;
                }

                var path = Application.LocalUserAppDataPath.Substring(
                    0, Application.LocalUserAppDataPath.LastIndexOf(@"\", StringComparison.Ordinal));

                var file = string.Format("log-{0}.txt", DateTime.Now.ToString("yyyy-MM-dd"));

                Process.Start(Path.Combine(path, "Logs", file));
            }
        }

        private static void MoveDatabaseIfExists()
        {
            var exeFolder = Path.GetDirectoryName(Application.ExecutablePath);

            var inputPath = Path.Combine(exeFolder, "Temp", "Database.sdf");

            if (exeFolder == null || !File.Exists(inputPath))
            {
                return;
            }

            var outputLocation = Path.Combine(Path.GetDirectoryName(Application.LocalUserAppDataPath), "Entities");
            var outputPath = Path.Combine(outputLocation, "Database.sdf");

            if (File.Exists(outputPath))
            {
                return;
            }

            Directory.CreateDirectory(outputLocation);
            File.Copy(inputPath, outputPath);
            File.Delete(inputPath);
        }

        private static void SetDefaultUserFolder()
        {
            var userFolderController = new UserFolderController(EntityFactory.Instance.Entities);

            var path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SimCity 4");

            if (!Directory.Exists(path) || userFolderController.UserFolders.Any(x => x.FolderPath.Equals(path)))
            {
                return;
            }

            Log.Info(string.Format("Setting default user folder to {0}", path));
            userFolderController.Add(new UserFolder(Guid.Empty) { Alias = LocalizationStrings.DefaultUserFolderName, FolderPath = path });
        }
    }
}
