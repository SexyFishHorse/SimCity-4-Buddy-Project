namespace NIHEI.SC4Buddy.View.UserFolders
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net.NetworkInformation;
    using System.Windows.Forms;

    using NIHEI.SC4Buddy.Control;
    using NIHEI.SC4Buddy.Control.Plugins;
    using NIHEI.SC4Buddy.Control.Remote;
    using NIHEI.SC4Buddy.Control.UserFolders;
    using NIHEI.SC4Buddy.DataAccess;
    using NIHEI.SC4Buddy.Entities;
    using NIHEI.SC4Buddy.Localization;
    using NIHEI.SC4Buddy.Properties;
    using NIHEI.SC4Buddy.Remote;
    using NIHEI.SC4Buddy.View.Elements;
    using NIHEI.SC4Buddy.View.Helpers;
    using NIHEI.SC4Buddy.View.Plugins;

    public partial class UserFolderForm : Form
    {
        private readonly UserFolder userFolder;

        private readonly PluginGroupController pluginGroupController;

        private readonly PluginController pluginController;

        private readonly UserFolderController userFolderController;

        private Plugin selectedPlugin;

        public UserFolderForm(
            PluginController pluginController,
            PluginGroupController pluginGroupController,
            UserFolderController userFolderController,
            UserFolder userFolder)
        {
            this.pluginGroupController = pluginGroupController;
            this.pluginController = pluginController;
            this.userFolderController = userFolderController;

            if (!Directory.Exists(userFolder.Path))
            {
                if (userFolder.Id == 1)
                {
                    if (userFolder.Alias.Equals("?"))
                    {
                        userFolder.Alias = LocalizationStrings.GameUserFolderName;
                    }

                    userFolder.Path = Settings.Default.GameLocation;
                    userFolderController.Update(userFolder);
                }
                else
                {
                    throw new DirectoryNotFoundException("The plugin folder does not exist.");
                }
            }

            this.userFolder = userFolder;
            InitializeComponent();
        }

        private void UserFolderFormLoad(object sender, EventArgs e)
        {
            RepopulateInstalledPluginsListView();

            if (!Settings.Default.EnableRemoteDatabaseConnection || !NetworkInterface.GetIsNetworkAvailable())
            {
                updateInfoForAllPluginsFromServerToolStripMenuItem.Visible = false;
                checkForMissingDependenciesToolStripMenuItem.Visible = false;
            }
            else
            {
                updateInfoForAllPluginsFromServerToolStripMenuItem.Visible = Settings.Default.FetchInfoFromRemote;
                checkForMissingDependenciesToolStripMenuItem.Visible = Settings.Default.AllowDependencyCheck;
            }
        }

        private void RepopulateInstalledPluginsListView()
        {
            installedPluginsListView.BeginUpdate();
            installedPluginsListView.Items.Clear();
            installedPluginsListView.Groups.Clear();

            foreach (var pluginGroup in pluginGroupController.Groups.Where(x => x.Plugins.Any()))
            {
                installedPluginsListView.Groups.Add(pluginGroup.Id.ToString(CultureInfo.InvariantCulture), pluginGroup.Name);
            }

            foreach (var plugin in
                pluginController.Plugins.Where(x => x.UserFolderId == userFolder.Id))
            {
                var listViewItem = new PluginListViewItem(
                    plugin,
                    installedPluginsListView.Groups[plugin.PluginGroupId.ToString()]);

                installedPluginsListView.Items.Add(listViewItem);
            }

            installedPluginsListView.EndUpdate();
        }

        private void InstalledPluginsListViewSelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedItems = installedPluginsListView.SelectedItems;

            if (selectedItems.Count > 0)
            {
                splitContainer1.Panel2Collapsed = false;
                selectedPlugin = ((PluginListViewItem)selectedItems[0]).Plugin;
                nameLabel.Text = selectedPlugin.Name;
                authorLabel.Text = selectedPlugin.Author;
                linkLabel.Text = selectedPlugin.Link;
                descriptionRichTextBox.Text = selectedPlugin.Description;

                uninstallButton.Enabled = true;
                updateInfoButton.Enabled = selectedPlugin.RemotePluginId == null;
                moveOrCopyButton.Enabled = true;
                disableFilesButton.Enabled = true;
            }
            else
            {
                splitContainer1.Panel2Collapsed = true;

                uninstallButton.Enabled = false;
                updateInfoButton.Enabled = false;
                moveOrCopyButton.Enabled = false;
                disableFilesButton.Enabled = false;
            }
        }

        private void UninstallButtonClick(object sender, EventArgs e)
        {
            var confirmation = MessageBox.Show(
                this,
                string.Format(LocalizationStrings.AreYouSureYouWantToUninstall, selectedPlugin.Name),
                LocalizationStrings.ConfirmUninstall,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);

            if (confirmation == DialogResult.No)
            {
                return;
            }

            userFolderController.UninstallPlugin(selectedPlugin);
            ClearPluginInformation();
            RepopulateInstalledPluginsListView();
        }

        private void ClearPluginInformation()
        {
            nameLabel.Text = string.Empty;
            authorLabel.Text = string.Empty;
            descriptionRichTextBox.Text = string.Empty;

            uninstallButton.Enabled = false;
            updateInfoButton.Enabled = false;
            splitContainer1.Panel2Collapsed = true;
        }

        private void CloseButtonClick(object sender, EventArgs e)
        {
            Close();
        }

        private void LinkLabelLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(selectedPlugin.Link);
        }

        private void UpdateInfoButtonClick(object sender, EventArgs e)
        {
            var infoDialog = new EnterPluginInformationForm(pluginGroupController) { Plugin = selectedPlugin };
            if (infoDialog.ShowDialog(this) == DialogResult.OK)
            {
                pluginController.SaveChanges();
            }

            RepopulateInstalledPluginsListView();
        }

        private void InstallToolStripMenuItemClick(object sender, EventArgs e)
        {
            var result = selectFilesToInstallDialog.ShowDialog(this);
            if (result == DialogResult.Cancel)
            {
                return;
            }

            var files = selectFilesToInstallDialog.FileNames;

            var confirmResult = MessageBox.Show(
                this,
                string.Format(LocalizationStrings.AreYouSureYouWantToInstallTheSelectedPlugins, files.Count()),
                LocalizationStrings.ConfirmInstallation,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button1);

            if (confirmResult == DialogResult.No)
            {
                return;
            }

            new InstallPluginsForm(pluginController, files, userFolder).ShowDialog(this);

            RepopulateInstalledPluginsListView();

            if (!NetworkInterface.GetIsNetworkAvailable() || !Settings.Default.EnableRemoteDatabaseConnection
                || !Settings.Default.AllowDependencyCheck)
            {
                return;
            }

            var scanForDependencies = MessageBox.Show(
                this,
                LocalizationStrings.WouldYouLikeToScanForMissingDependencies,
                LocalizationStrings.DependencyCheck,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1);

            if (scanForDependencies == DialogResult.Yes)
            {
                CheckForMissingDependenciesToolStripMenuItemClick(sender, e);
            }
        }

        private void ScanForNewPluginsToolStripMenuItemClick(object sender, EventArgs e)
        {
            new FolderScannerForm(
                pluginController,
                pluginGroupController,
                new PluginFileController(EntityFactory.Instance.Entities),
                userFolder).ShowDialog(this);
            RepopulateInstalledPluginsListView();

            if (!NetworkInterface.GetIsNetworkAvailable() || !Settings.Default.EnableRemoteDatabaseConnection
                || !Settings.Default.AllowDependencyCheck)
            {
                return;
            }

            var scanForDependencies = MessageBox.Show(
                this,
                LocalizationStrings.WouldYouLikeToScanForMissingDependencies,
                LocalizationStrings.DependencyCheck,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1);

            if (scanForDependencies == DialogResult.Yes)
            {
                CheckForMissingDependenciesToolStripMenuItemClick(sender, e);
            }
        }

        private void ScanForNonpluginFilesToolStripMenuItemClick(object sender, EventArgs e)
        {
            int numFiles;
            int numFolders;

            var scanner = new NonPluginFilesScanner();
            scanner.HasFilesAndFoldersToRemove(userFolder, out numFiles, out numFolders);

            if (numFiles < 1 && numFolders < 1)
            {
                NonPluginFilesScannerUiHelper.ShowThereAreNoEntitiesToRemoveDialog(this);
                return;
            }

            if (!NonPluginFilesScannerUiHelper.ShowConfirmDialog(this, numFiles, numFolders))
            {
                return;
            }

            numFolders = scanner.RemoveNonPluginFiles(userFolder);

            NonPluginFilesScannerUiHelper.ShowRemovalSummary(this, numFiles, numFolders);
        }

        private void UpdateInfoForAllPluginsFromServerToolStripMenuItemClick(object sender, EventArgs e)
        {
            var numUpdated = userFolderController.UpdateInfoForAllPluginsFromServer();
            RepopulateInstalledPluginsListView();

            MessageBox.Show(
                this,
                string.Format(LocalizationStrings.InformationForNumPluginsWereUpdated, numUpdated),
                LocalizationStrings.PluginInformationUpdated,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1);
        }

        private void UserFolderFormActivated(object sender, EventArgs e)
        {
            updateInfoForAllPluginsFromServerToolStripMenuItem.Visible = Settings.Default.FetchInfoFromRemote;
        }

        private void CheckForMissingDependenciesToolStripMenuItemClick(object sender, EventArgs e)
        {
            userFolderController.UpdateInfoForAllPluginsFromServer();

            var numRecognizedPlugins = userFolderController.NumberOfRecognizedPlugins(userFolder);

            if (numRecognizedPlugins < 1)
            {
                MessageBox.Show(
                    this,
                    LocalizationStrings.NoneOfYourPluginsAreRecognizedOnTheCentralServerAndCanThereforeNotBeChecked,
                    LocalizationStrings.NoRecognizablePluginsFound,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1);
                return;
            }

            var checker = new DependencyChecker(
                userFolderController, new RemotePluginController(EntityFactory.Instance.RemoteEntities));
            var missingDependencies = checker.CheckDependencies(userFolder);

            if (missingDependencies.Any())
            {
                var dialog = new MissingDependenciesForm { MissingDependencies = missingDependencies };
                dialog.ShowDialog(this);
            }
            else
            {
                MessageBox.Show(
                    this,
                    string.Format(
                        LocalizationStrings.NumPluginsCheckedForMissingPluginsAndNoneWereMissing,
                        numRecognizedPlugins),
                    LocalizationStrings.NoDependenciesMissing,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1);
            }
        }

        private void MoveOrCopyButtonClick(object sender, EventArgs e)
        {
            var dialog = new MoveOrCopyForm(
                userFolder,
                userFolderController,
                pluginController,
                new PluginFileController(EntityFactory.Instance.Entities)) { Plugin = selectedPlugin };
            dialog.PluginCopied += DialogOnPluginCopied;
            dialog.PluginMoved += DialogOnPluginMoved;
            dialog.ErrorDuringCopyOrMove += DialogOnErrorDuringCopyOrMove;
            dialog.ShowDialog(this);

            RepopulateInstalledPluginsListView();
        }

        private void DialogOnPluginMoved(object sender, EventArgs eventArgs)
        {
            MessageBox.Show(
                this,
                LocalizationStrings.PluginHasBeenSuccessfullyMoved,
                LocalizationStrings.PluginMoved,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void DialogOnErrorDuringCopyOrMove(object sender, EventArgs eventArgs)
        {
            MessageBox.Show(
                this,
                LocalizationStrings.ErrorDuringCopyOrMoveOperation,
                LocalizationStrings.ErrorDuringCopyingOrMovingPlugin,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private void DialogOnPluginCopied(object sender, EventArgs eventArgs)
        {
            MessageBox.Show(
                this,
                LocalizationStrings.PluginHasBeenSuccessfullyCopied,
                LocalizationStrings.PluginCopied,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void DisableFilesButtonClick(object sender, EventArgs e)
        {
            var dialog = new QuarantinedPluginFilesForm(
                selectedPlugin,
                pluginController,
                new PluginFileController(EntityFactory.Instance.Entities));
            var result = dialog.ShowDialog(this);
        }
    }
}
