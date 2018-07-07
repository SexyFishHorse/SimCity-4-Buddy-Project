namespace Nihei.SC4Buddy
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Forms;
    using Asser.Sc4Buddy.Server.Api.V1.Client;
    using log4net;
    using log4net.Config;
    using Nihei.SC4Buddy.Application.Control;
    using Nihei.SC4Buddy.Application.Utilities;
    using Nihei.SC4Buddy.Application.View;
    using Nihei.SC4Buddy.Configuration;
    using Nihei.SC4Buddy.DataAccess;
    using Nihei.SC4Buddy.Model;
    using Nihei.SC4Buddy.Plugins.Control;
    using Nihei.SC4Buddy.Plugins.Services;
    using Nihei.SC4Buddy.Resources;
    using Nihei.SC4Buddy.UserFolders.Control;
    using Nihei.SC4Buddy.UserFolders.DataAccess;
    using Nihei.SC4Buddy.Utils;
    using RestSharp;

    public static class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        [STAThread]
        public static void Main(string[] args)
        {
            XmlConfigurator.Configure();

            Log.Info("Application starting");

            var exceptionHandling = true;
            if (args != null)
            {
                exceptionHandling = args.All(arg => arg != "-exceptionHandling:off");
                Log.Warn("Exception handling disabled by command line argument.");
            }

            try
            {
                var entities = EntityFactory.Instance.Entities;
                var userFolderController = new UserFolderController(new UserFolderDataAccess(new JsonFileWriter()));
                var userFoldersController = new UserFoldersController(new UserFoldersDataAccess(new JsonFileWriter()), userFolderController);

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
                    var buddyServerClient = new BuddyServerClient(
                        new RestClient(Settings.Get(Settings.Keys.ApiBaseUrl, "http://api.sc4buddy.sexyfishhorse.com")));

                    System.Windows.Forms.Application.Run(
                        new Sc4Buddy(
                            userFoldersController,
                            new PluginGroupController(entities),
                            new PluginMatcher(buddyServerClient)));
                }
            }
            catch (Exception ex)
            {
                Log.Error("Uncaught error", ex);

                if (!exceptionHandling)
                {
                    throw;
                }

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
            var userFoldersController = new UserFoldersController(
                new UserFoldersDataAccess(new JsonFileWriter()),
                new UserFolderController(new UserFolderDataAccess(new JsonFileWriter())));

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
