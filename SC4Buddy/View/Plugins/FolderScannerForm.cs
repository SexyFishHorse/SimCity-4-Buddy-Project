namespace NIHEI.SC4Buddy.View.Plugins
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Security.Policy;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    using Irradiated.Sc4Buddy.ApiClient.Model;

    using log4net;

    using NIHEI.Common.IO;
    using NIHEI.SC4Buddy.Control.Plugins;
    using NIHEI.SC4Buddy.Localization;
    using NIHEI.SC4Buddy.Model;
    using NIHEI.SC4Buddy.Remote;
    using NIHEI.SC4Buddy.View.Elements;
    using Plugin = NIHEI.SC4Buddy.Model.Plugin;
    using PluginFile = NIHEI.SC4Buddy.Model.PluginFile;

    public partial class FolderScannerForm : Form
    {
        private const int ErrorIconPadding = -18;

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly FolderScannerController folderScannerController;

        private readonly PluginController pluginController;

        private readonly PluginGroupController pluginGroupController;

        private readonly IPluginMatcher pluginMatcher;

        private readonly UserFolder userFolder;

        public FolderScannerForm(
            FolderScannerController folderScannerController,
            PluginController pluginController,
            PluginGroupController pluginGroupController,
            UserFolder userFolder,
            IPluginMatcher pluginMatcher)
        {
            this.folderScannerController = folderScannerController;

            this.pluginController = pluginController;

            this.pluginGroupController = pluginGroupController;

            this.userFolder = userFolder;
            this.pluginMatcher = pluginMatcher;

            InitializeComponent();

            fileScannerBackgroundWorker.DoWork += ScanFolder;
            fileScannerBackgroundWorker.ProgressChanged += FileScannerBackgroundWorkerOnProgressChanged;
            fileScannerBackgroundWorker.RunWorkerCompleted += delegate
                {
                    statusProgressBar.Visible = false;
                    statusLabel.Visible = false;
                    scanButton.Enabled = true;
                };

            folderScannerController.NewFilesFound += FolderScannerControllerOnNewFilesFound;
        }

        private void FolderScannerControllerOnNewFilesFound(object sender, EventArgs eventArgs)
        {
            Invoke(new MethodInvoker(RepopulateNewFilesListView));
        }

        private void RepopulateNewFilesListView()
        {
            newFilesListView.BeginUpdate();
            newFilesListView.Items.Clear();

            foreach (var file in folderScannerController.NewFiles)
            {
                if (fileScannerBackgroundWorker.CancellationPending)
                {
                    return;
                }

                var filename = file.Remove(0, userFolder.PluginFolderPath.Length + 1);
                newFilesListView.Items.Add(filename);
            }

            ResizeColumns();

            newFilesListView.EndUpdate();

            if (!folderScannerController.NewFiles.Any())
            {
                return;
            }

            addAllButton.Enabled = true;
            autoGroupKnownPlugins.Enabled = true;
        }

        private void FileScannerBackgroundWorkerOnProgressChanged(
            object sender,
            ProgressChangedEventArgs progressChangedEventArgs)
        {
            statusProgressBar.Value = progressChangedEventArgs.ProgressPercentage;
        }

        private void ScanFolder(object sender, EventArgs e)
        {
            if (!folderScannerController.ScanFolder(userFolder))
            {
                Invoke(
                    new MethodInvoker(
                        () =>
                        {
                            MessageBox.Show(
                                this,
                                LocalizationStrings.NoNewDeletedOrUpdatedFilesDetected,
                                LocalizationStrings.NoFileChangesDetected,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information,
                                MessageBoxDefaultButton.Button1);
                            Close();
                        }));
            }
        }

        private void NewFilesListViewSelectedIndexChanged(object sender, EventArgs e)
        {
            if (newFilesListView.SelectedItems.Count > 0)
            {
                addButton.Enabled = true;
                addAllButton.Enabled = true;
            }
            else
            {
                addButton.Enabled = false;
                addAllButton.Enabled = false;
            }
        }

        private void PluginFilesListViewSelectedIndexChanged(object sender, EventArgs e)
        {
            if (pluginFilesListView.SelectedItems.Count > 0)
            {
                removeButton.Enabled = true;
                removeAllButton.Enabled = true;
            }
            else
            {
                removeButton.Enabled = false;
                addAllButton.Enabled = false;
            }

            ValidatePluginInfo(false);
        }

        private void AddButtonClick(object sender, EventArgs e)
        {
            var items = newFilesListView.SelectedItems;

            if (items.Count <= 0)
            {
                return;
            }

            if (pluginFilesListView.Items.Count == 0 && nameTextBox.Text.Length < 1)
            {
                var path = items[0].Text;
                var fileInfo = new FileInfo(path);

                nameTextBox.Text = fileInfo.Name;
            }

            MoveItemsToPluginFilesListView(items);

            removeAllButton.Enabled = true;

            NewFilesListViewSelectedIndexChanged(sender, e);
        }

        private void AddAllButtonClick(object sender, EventArgs e)
        {
            var items = newFilesListView.Items;

            if (items.Count <= 0)
            {
                return;
            }

            if (pluginFilesListView.Items.Count == 0 && nameTextBox.Text.Length < 1)
            {
                var path = items[0].Text;
                var fileInfo = new FileInfo(path);

                nameTextBox.Text = fileInfo.Name;
            }

            MoveItemsToPluginFilesListView(items);

            removeAllButton.Enabled = true;
            addButton.Enabled = false;
            addAllButton.Enabled = false;
        }

        private void MoveItemsToPluginFilesListView(IEnumerable items)
        {
            pluginFilesListView.BeginUpdate();
            newFilesListView.BeginUpdate();

            foreach (ListViewItem item in items)
            {
                newFilesListView.Items.Remove(item);
                pluginFilesListView.Items.Add(item);
            }

            ResizeColumns();

            newFilesListView.EndUpdate();
            pluginFilesListView.EndUpdate();
        }

        private void RemoveButtonClick(object sender, EventArgs e)
        {
            var items = pluginFilesListView.SelectedItems;

            if (items.Count > 0)
            {
                pluginFilesListView.BeginUpdate();
                newFilesListView.BeginUpdate();

                foreach (ListViewItem item in items)
                {
                    pluginFilesListView.Items.Remove(item);
                    newFilesListView.Items.Add(item);
                }

                ResizeColumns();

                newFilesListView.EndUpdate();
                pluginFilesListView.EndUpdate();

                addAllButton.Enabled = true;

                PluginFilesListViewSelectedIndexChanged(sender, e);
            }
        }

        private void RemoveAllButtonClick(object sender, EventArgs e)
        {
            var items = pluginFilesListView.Items;

            if (items.Count <= 0)
            {
                return;
            }

            pluginFilesListView.BeginUpdate();
            newFilesListView.BeginUpdate();

            foreach (ListViewItem item in items)
            {
                pluginFilesListView.Items.Remove(item);
                newFilesListView.Items.Add(item);
            }

            ResizeColumns();

            newFilesListView.EndUpdate();
            pluginFilesListView.EndUpdate();

            addAllButton.Enabled = true;
            removeButton.Enabled = false;
            removeAllButton.Enabled = false;
        }

        private void ResizeColumns()
        {
            newFilesListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            pluginFilesListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void ScanButtonClick(object sender, EventArgs e)
        {
            try
            {
                fileScannerBackgroundWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                Log.Error("Error during folder scanner", ex);

                // TODO: reload entities
            }

            statusProgressBar.Visible = true;
            statusLabel.Text = LocalizationStrings.ScandingFolderThisMayTakeAFewMinutesIfYouHaveAVeryLargePluginFolder;
            statusLabel.Visible = true;
            scanButton.Enabled = false;
        }

        private void SaveButtonClick(object sender, EventArgs e)
        {
            if (!ValidatePluginInfo())
            {
                return;
            }

            var linkString = linkTextBox.Text.Trim();
            Url link = null;
            if (!string.IsNullOrWhiteSpace(linkString))
            {
                link = new Url(linkString);
            }

            var plugin = new Plugin
            {
                Name = nameTextBox.Text.Trim(),
                Author = authorTextBox.Text.Trim(),
                Link = link,
                PluginGroup = GetSelectedGroup(),
                Description = descriptionTextBox.Text.Trim(),
                UserFolder = userFolder
            };

            var group = plugin.PluginGroup;
            if (group != null)
            {
                group.Plugins.Add(plugin);
            }

            foreach (var pluginFile in
                pluginFilesListView.Items.Cast<ListViewItem>().Select(item => item.Text).Select(
                    path => new PluginFile
                    {
                        Path = Path.Combine(userFolder.PluginFolderPath, path),
                        Checksum =
                            Md5ChecksumUtility.CalculateChecksum(Path.Combine(userFolder.PluginFolderPath, path))
                                .ToHex(),
                        Plugin = plugin
                    }))
            {
                plugin.PluginFiles.Add(pluginFile);
            }

            pluginController.Add(plugin);

            ClearInfoAndSelectedFilesForms();

            if (newFilesListView.Items.Count > 0)
            {
                addAllButton.Enabled = true;
            }
        }

        private void ClearInfoAndSelectedFilesForms()
        {
            nameTextBox.Text = string.Empty;
            authorTextBox.Text = string.Empty;
            linkTextBox.Text = string.Empty;
            descriptionTextBox.Text = string.Empty;
            groupComboBox.Text = string.Empty;
            saveButton.Enabled = false;
            pluginFilesListView.Items.Clear();

            errorProvider1.Clear();
            removeButton.Enabled = false;
            removeAllButton.Enabled = false;
        }

        private bool ValidatePluginInfo(bool showErrors = true)
        {
            var errors = false;
            if (string.IsNullOrWhiteSpace(nameTextBox.Text))
            {
                if (showErrors)
                {
                    errorProvider1.SetIconPadding(nameTextBox, ErrorIconPadding);
                    errorProvider1.SetError(nameTextBox, "You must enter a name for the plugin");
                }

                errors = true;
            }

            if (pluginFilesListView.Items.Count < 1)
            {
                if (showErrors)
                {
                    errorProvider1.SetIconPadding(pluginFilesListView, ErrorIconPadding);
                    errorProvider1.SetError(pluginFilesListView, "You must select at least one file.");
                }

                errors = true;
            }

            saveButton.Enabled = !errors;

            return !errors;
        }

        private PluginGroup GetSelectedGroup()
        {
            var selectedText = groupComboBox.Text;

            if (string.IsNullOrWhiteSpace(selectedText))
            {
                return null;
            }

            var group =
                pluginGroupController.Groups.FirstOrDefault(
                    x => x.Name.Equals(selectedText, StringComparison.OrdinalIgnoreCase));

            if (group == null)
            {
                group = new PluginGroup
                {
                    Name = selectedText.Trim()
                };
                pluginGroupController.Add(group);
            }

            return group;
        }

        private void FolderScannerFormLoad(object sender, EventArgs e)
        {
            groupComboBox.BeginUpdate();

            foreach (var pluginGroup in pluginGroupController.Groups)
            {
                groupComboBox.Items.Add(new ComboBoxItem<PluginGroup>(pluginGroup.Name, pluginGroup));
            }

            groupComboBox.EndUpdate();
        }

        private void NameTextBoxTextChanged(object sender, EventArgs e)
        {
            ValidatePluginInfo(false);
        }

        private async Task<bool> AutoGroupKnownPlugins()
        {
            try
            {
                statusProgressBar.Visible = true;
                statusLabel.Text =
                    LocalizationStrings
                        .TryingToAutoGroupPluginsThisMayTakeAFewMinutesIfYouHaveALargePluginFolderOrASlowInternetConnection;
                statusLabel.Visible = true;
                autoGroupKnownPlugins.Enabled = false;
                await folderScannerController.AutoGroupKnownFiles(userFolder, pluginController, pluginMatcher);
                RepopulateNewFilesListView();
            }
            catch (Sc4BuddyClientException ex)
            {
                Log.Warn("Api error during auto group known plugins", ex);
                var result = MessageBox.Show(
                    this,
                    string.Format(LocalizationStrings.AnErrorOccuredWhenTryingToAutoGroupPlugins, ex.Message),
                    LocalizationStrings.ErrorDuringAutoGroupingOfPlugins,
                    MessageBoxButtons.RetryCancel,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Retry)
                {
                    AutoGroupKnownPlugins();
                }
            }
            finally
            {
                statusProgressBar.Visible = false;
                statusLabel.Text = string.Empty;
                statusLabel.Visible = true;
                autoGroupKnownPlugins.Enabled = true;
            }

            return true;
        }

        private async void AutoGroupKnownPluginsClick(object sender, EventArgs e)
        {
            try
            {
                await AutoGroupKnownPlugins();
            }
            catch (Exception ex)
            {
                Log.Error("Auto group known plugins error", ex);
                MessageBox.Show(
                    this,
                    LocalizationStrings.ErrorOccuredDuringAutoGroupKnownPlugins + ex.Message,
                    LocalizationStrings.ErrorDuringAutoGroupKnownPlugins,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private void CloseButtonClick(object sender, EventArgs e)
        {
            try
            {
                fileScannerBackgroundWorker.CancelAsync();
            }
            catch (Exception ex)
            {
                Log.Warn("Exception during folder scanner form close", ex);
            }
        }
    }
}
