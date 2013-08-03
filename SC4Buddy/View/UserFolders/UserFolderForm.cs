namespace NIHEI.SC4Buddy.View.UserFolders
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;

    using NIHEI.SC4Buddy.Control.UserFolders;
    using NIHEI.SC4Buddy.DataAccess;
    using NIHEI.SC4Buddy.DataAccess.Plugins;
    using NIHEI.SC4Buddy.Entities;
    using NIHEI.SC4Buddy.Localization;
    using NIHEI.SC4Buddy.Properties;
    using NIHEI.SC4Buddy.Remote;
    using NIHEI.SC4Buddy.View.Elements;
    using NIHEI.SC4Buddy.View.Plugins;

    public partial class UserFolderForm : Form
    {
        private readonly UserFolder userFolder;

        private readonly PluginGroupRegistry groupRegistry;

        private readonly PluginRegistry pluginRegistry;

        private readonly UserFolderController controller;

        private Plugin selectedPlugin;

        public UserFolderForm(UserFolder userFolder)
        {
            groupRegistry = RegistryFactory.PluginGroupRegistry;
            pluginRegistry = RegistryFactory.PluginRegistry;
            controller = new UserFolderController(RegistryFactory.UserFolderRegistry);

            if (!Directory.Exists(userFolder.Path))
            {
                if (userFolder.Id == 1)
                {
                    if (userFolder.Alias.Equals("?"))
                    {
                        userFolder.Alias = LocalizationStrings.GameUserFolderName;
                    }

                    userFolder.Path = Settings.Default.GameLocation;
                    controller.Update(userFolder);
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
        }

        private void RepopulateInstalledPluginsListView()
        {
            installedPluginsListView.BeginUpdate();
            installedPluginsListView.Items.Clear();
            installedPluginsListView.Groups.Clear();

            foreach (var pluginGroup in groupRegistry.PluginGroups.Where(x => x.Plugins.Any()))
            {
                installedPluginsListView.Groups.Add(pluginGroup.Id.ToString(CultureInfo.InvariantCulture), pluginGroup.Name);
            }

            foreach (
                var listViewItem in
                    pluginRegistry.Plugins
                    .Where(x => x.UserFolderId == userFolder.Id)
                    .Select(
                        plugin =>
                        new PluginListViewItem(
                            plugin,
                            installedPluginsListView.Groups[plugin.PluginGroupId.ToString()])))
            {
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

                isRemoteInfoLabel.Visible = selectedPlugin.RemotePluginId != null;

                uninstallButton.Enabled = true;
                updateInfoButton.Enabled = selectedPlugin.RemotePluginId == null;
            }
            else
            {
                splitContainer1.Panel2Collapsed = true;

                uninstallButton.Enabled = false;
                updateInfoButton.Enabled = false;
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

            controller.UninstallPlugin(selectedPlugin);
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
            var infoDialog = new EnterPluginInformationForm { Plugin = selectedPlugin };
            if (infoDialog.ShowDialog(this) == DialogResult.OK)
            {
                pluginRegistry.Update(infoDialog.Plugin);
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
                MessageBoxDefaultButton.Button2);

            if (confirmResult == DialogResult.No)
            {
                return;
            }

            new InstallPluginsForm(files, userFolder).ShowDialog(this);

            RepopulateInstalledPluginsListView();

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
            new FolderScannerForm(userFolder).ShowDialog(this);
            RepopulateInstalledPluginsListView();
        }

        private void ScanForNonpluginFilesToolStripMenuItemClick(object sender, EventArgs e)
        {
            int numFiles;
            int numFolders;

            controller.GetEstimatedFilesAndFoldersToBeRemoved(userFolder, out numFiles, out numFolders);

            if (numFiles < 1 && numFolders < 1)
            {
                MessageBox.Show(
                    this,
                    LocalizationStrings.ThereAreNoNonPluginFilesOrEmptyFoldersToRemove,
                    LocalizationStrings.NoNonPluginFilesDetected,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            var confirmation = MessageBox.Show(
                this,
                string.Format(LocalizationStrings.ThisWillRemoveNumFilesAndAtLeastNumFolders, numFiles, numFolders),
                LocalizationStrings.ConfirmDeletionOfNonPluginFiles,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);

            if (confirmation == DialogResult.No)
            {
                return;
            }

            numFolders = controller.RemoveNonPluginFiles(userFolder);

            var message = string.Format(LocalizationStrings.NumFilesAndNumFoldersWereRemoved, numFiles, numFolders);

            MessageBox.Show(
                this,
                message,
                LocalizationStrings.NonPluginFilesDeleted,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1);
        }

        private void UpdateInfoForAllPluginsFromServerToolStripMenuItemClick(object sender, EventArgs e)
        {
            var numUpdated = controller.UpdateInfoForAllPluginsFromServer();
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
            controller.UpdateInfoForAllPluginsFromServer();

            var numRecognizedPlugins = controller.NumberOfRecognizedPlugins(userFolder);

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

            var checker = new DependencyChecker();
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
                    string.Format(LocalizationStrings.NumPluginsCheckedForMissingPluginsAndNoneWereMissing, numRecognizedPlugins),
                    LocalizationStrings.NoDependenciesMissing,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1);
            }
        }
    }
}
