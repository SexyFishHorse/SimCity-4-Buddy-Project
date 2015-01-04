namespace NIHEI.SC4Buddy
{
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Forms;

    using Irradiated.Sc4Buddy.ApiClient;

    using log4net;
    using log4net.Config;
    using NIHEI.SC4Buddy.Application.Control;
    using NIHEI.SC4Buddy.Application.Utilities;
    using NIHEI.SC4Buddy.Application.View;
    using NIHEI.SC4Buddy.Configuration;
    using NIHEI.SC4Buddy.DataAccess;
    using NIHEI.SC4Buddy.Model;
    using NIHEI.SC4Buddy.Plugins.Control;
    using NIHEI.SC4Buddy.Remote;
    using NIHEI.SC4Buddy.Resources;
    using NIHEI.SC4Buddy.UserFolders.Control;
    using NIHEI.SC4Buddy.UserFolders.DataAccess;

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
                var entities = EntityFactory.Instance.Entities;
                var userFolderController = new UserFolderController(new PluginFileController(entities), new PluginController(entities), entities);
                var userFoldersController = new UserFoldersController(new UserFoldersDataAccess(), userFolderController);
                System.Windows.Forms.Application.EnableVisualStyles();
                System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
                System.Windows.Forms.Application.ApplicationExit += (sender, eventArgs) => Log.Info("Application exited");

                if (string.IsNullOrWhiteSpace(Settings.Get(Settings.Keys.GameLocation)) || !Directory.Exists(Settings.Get(Settings.Keys.GameLocation)))
                {
                    var settingsForm = new SettingsForm(userFoldersController) { StartPosition = FormStartPosition.CenterScreen };

                    System.Windows.Forms.Application.Run(settingsForm);
                    SetDefaultUserFolder();
                }

                if (userFoldersController.UserFolders.Any(x => x.IsMainFolder && x.FolderPath.Equals("?")))
                {
                    SetDefaultUserFolder();
                }

                if (Directory.Exists(Settings.Get(Settings.Keys.GameLocation)))
                {
                    new SettingsController(userFoldersController).CheckMainFolder();
                    System.Windows.Forms.Application.Run(
                        new Sc4Buddy(
                            userFoldersController,
                            new PluginController(entities),
                            new PluginGroupController(entities),
                            new PluginMatcher(
                                new Sc4BuddyApiClient(ConfigurationManager.AppSettings["ApiBaseUrl"], string.Empty)),
                            new DependencyChecker(new Sc4BuddyApiClient(ConfigurationManager.AppSettings["ApiBaseUrl"], string.Empty), userFoldersController.GetMainUserFolder()),
                            new PluginFileController(entities),
                            entities));
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

                var file = string.Format("log-{0}.txt", DateTime.Now.ToString("yyyy-MM-dd"));

                Process.Start(Path.Combine(FileSystemLocationsUtil.LogFilesDirectory, file));
            }
        }

        private static void SetDefaultUserFolder()
        {
            var entities = EntityFactory.Instance.Entities;
            var userFoldersController = new UserFoldersController(
                new UserFoldersDataAccess(),
                new UserFolderController(new PluginFileController(entities), new PluginController(entities), entities));

            var path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SimCity 4");

            if (!Directory.Exists(path) || userFoldersController.UserFolders.Any(x => x.FolderPath.Equals(path)))
            {
                return;
            }

            Log.Info(string.Format("Setting default user folder to {0}", path));
            userFoldersController.Add(new UserFolder { Alias = LocalizationStrings.DefaultUserFolderName, FolderPath = path });
        }
    }
}
