namespace NIHEI.SC4Buddy.View.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.NetworkInformation;
    using System.Threading;
    using System.Windows.Forms;

    using NIHEI.SC4Buddy.DataAccess;
    using NIHEI.SC4Buddy.DataAccess.Plugins;
    using NIHEI.SC4Buddy.Entities;
    using NIHEI.SC4Buddy.Installer;
    using NIHEI.SC4Buddy.Installer.InstallerEventArgs;
    using NIHEI.SC4Buddy.Localization;
    using NIHEI.SC4Buddy.Properties;
    using NIHEI.SC4Buddy.Remote;

    public partial class InstallPluginsForm : Form
    {
        private readonly IList<string> installedPlugins;

        private readonly IList<string> failedPlugins;

        private readonly IList<Plugin> tempPluginInfo;

        private readonly PluginRegistry pluginRegistry;

        private readonly EnterPluginInformationForm enterPluginInformationForm;

        public InstallPluginsForm(string[] files, UserFolder userFolder)
        {
            InitializeComponent();

            installedPlugins = new List<string>();
            failedPlugins = new List<string>();
            tempPluginInfo = new List<Plugin>();

            enterPluginInformationForm = new EnterPluginInformationForm();

            pluginRegistry = RegistryFactory.PluginRegistry;

            OverallProgressBar.Maximum = files.Length;
            CurrentProgressBar.Maximum = 100;

            var pluginInstallerThread = new PluginInstallerThread
                                            {
                                                Form = this,
                                                FilesToInstall = files,
                                                UserFolder = userFolder
                                            };
            pluginInstallerThread.InstallingPlugin += OnInstallingPlugin;
            pluginInstallerThread.PluginInstalled += OnPluginInstalled;
            pluginInstallerThread.PluginInstallFailed += OnPluginInstallFailed;
            pluginInstallerThread.InstallProgressChanged += OnInstallProgressChanged;
            pluginInstallerThread.AllPluginsInstalled += OnAllPluginsInstalled;
            pluginInstallerThread.ReadmeFilesFound += OnReadmeFilesFound;
            pluginInstallerThread.NotPartOneOfMultipartDetected += OnNotPartOneOfMultipartDetected;

            var installerThread = new Thread(pluginInstallerThread.Install);
            installerThread.SetApartmentState(ApartmentState.STA);
            installerThread.Start();
        }

        private void OnNotPartOneOfMultipartDetected(PluginInstallerThread sender, InstallPluginEventArgs args)
        {
            WriteLine(string.Format(LocalizationStrings.FileXSkipped, args.FileInfo.Name));
            WriteLine(LocalizationStrings.YouCanOnlySelectPart1OfAMultipartPlugin);
            IncrementInstalledPlugins();
        }

        public bool AskToRunExecutable(UserFolder userFolder, FileInfo executable)
        {
            return (bool)Invoke(new Func<bool>(() => ShowAskToRunExecutable(userFolder, executable)));
        }

        public void ShowInstallationDidNotStartDialog()
        {
            Invoke(
                new Action(
                    () =>
                    MessageBox.Show(
                        this,
                        LocalizationStrings.ItSeemsLikeTheInstallationDidNotStart,
                        LocalizationStrings.PossibleErrorDetected,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Stop)));
        }

        protected void IncrementInstalledPlugins()
        {
            SetOverallProgresBar(OverallProgressBar.Value + 1);
            SetCurrentProgressBar(100);
        }

        protected void ResetCurrentProgressBar()
        {
            CurrentProgressBar.BeginInvoke(new Action(
                () => CurrentProgressBar.Value = 0));
        }

        protected void SetOverallProgresBar(int value)
        {
            OverallProgressBar.BeginInvoke(new Action(
                () => OverallProgressBar.Value = value));
        }

        protected void SetCurrentProgressBar(int value)
        {
            CurrentProgressBar.BeginInvoke(new Action(
                () => CurrentProgressBar.Value = value));
        }

        protected void Write(string text)
        {
            OutputTextBox.BeginInvoke(new Action(
                () => OutputTextBox.AppendText(text)));
        }

        protected void WriteLine(string message)
        {
            Write("\n" + message);
        }

        private bool ShowAskToRunExecutable(UserFolder userFolder, FileInfo executable)
        {
            var messageString = string.Format(
                LocalizationStrings.DoYouWantToRunTheNamExecutable,
                new object[] { executable.Name, Path.Combine(userFolder.Path, "Plugins") });

            return MessageBox.Show(
                this,
                messageString,
                LocalizationStrings.RunExecutable,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button1) == DialogResult.Yes;
        }

        private void OnReadmeFilesFound(PluginInstallerThread sender, ReadmeFilesEventArgs args)
        {
            Invoke(new Action(() => new ReadmeFilesForm(args.ReadmeFiles).ShowDialog(this)));
        }

        private void OnAllPluginsInstalled(object sender, EventArgs eventArgs)
        {
            WriteLine(string.Empty);
            WriteLine(LocalizationStrings.InstallationCompleted);
            WriteLine(string.Format(LocalizationStrings.NumPluginsInstalledSuccessfully, installedPlugins.Count));

            if (failedPlugins.Any())
            {
                WriteLine(string.Format(LocalizationStrings.NumPluginsFailedInstallationPleaseReview, failedPlugins.Count));
                WriteLine(LocalizationStrings.FailedPlugins);
                foreach (var failedPlugin in failedPlugins)
                {
                    WriteLine(failedPlugin);
                }
            }

            if (tempPluginInfo.Any())
            {
                Invoke(new Action(() =>
                    {
                        if (NetworkInterface.GetIsNetworkAvailable() && Settings.Default.EnableRemoteDatabaseConnection && Settings.Default.FetchInfoFromRemote)
                        {
                            var matcher = new PluginMatcher();
                            var matched = tempPluginInfo.Where(matcher.MatchAndUpdate).ToList();

                            foreach (var match in matched)
                            {
                                tempPluginInfo.Remove(match);
                            }
                        }

                        if (!tempPluginInfo.Any())
                        {
                            return;
                        }

                        if (!Settings.Default.InstallerAskForAdditionalInfo || ShowWouldYouLikeToEnterAdditionalDetailsDialog() == DialogResult.No)
                        {
                            return;
                        }

                        foreach (var plugin in tempPluginInfo)
                        {
                            enterPluginInformationForm.Plugin = plugin;
                            var result = ShowEnterPluginInformationForm();
                            if (result == DialogResult.OK)
                            {
                                pluginRegistry.Update(enterPluginInformationForm.Plugin);
                            }
                        }
                    }));
            }

            Invoke(new Action(() => closeButton.Enabled = true));
        }

        private DialogResult ShowEnterPluginInformationForm()
        {
            return enterPluginInformationForm.ShowDialog(this);
        }

        private DialogResult ShowWouldYouLikeToEnterAdditionalDetailsDialog()
        {
            return MessageBox.Show(
                this,
                LocalizationStrings.WouldYouLikeToEnterAdditionalDetailsForTheInstalledPlugins,
                LocalizationStrings.EnterAdditionalPluginInformation,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button1);
        }

        private void OnInstallProgressChanged(PluginInstallerThread sender, InstallProgressEventArgs e)
        {
            WriteLine(e.Message);
            SetCurrentProgressBar(e.Progress);
        }

        private void OnPluginInstallFailed(PluginInstallerThread sender, InstallPluginFailureEventArgs e)
        {
            WriteLine(e.ErrorMessage);
            WriteLine(LocalizationStrings.PluginInstallFailed);
            IncrementInstalledPlugins();
            failedPlugins.Add(e.FileInfo.Name);
        }

        private void OnPluginInstalled(PluginInstallerThread sender, InstalledPluginEventArgs e)
        {
            WriteLine(LocalizationStrings.PluginInstalledSuccessfully);
            SetCurrentProgressBar(100);
            IncrementInstalledPlugins();
            installedPlugins.Add(e.FileInfo.Name);
            tempPluginInfo.Add(e.Plugin);
        }

        private void OnInstallingPlugin(PluginInstallerThread sender, InstallPluginEventArgs e)
        {
            WriteLine(string.Format(LocalizationStrings.InstallingPluginNam, e.FileInfo.Name));
            ResetCurrentProgressBar();
        }

        private void CloseButtonClick(object sender, EventArgs e)
        {
            Close();
        }
    }
}
