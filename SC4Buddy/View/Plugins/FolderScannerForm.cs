namespace NIHEI.SC4Buddy.View.Plugins
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;

    using NIHEI.Common.IO;
    using NIHEI.SC4Buddy.Control.UserFolders;
    using NIHEI.SC4Buddy.DataAccess;
    using NIHEI.SC4Buddy.DataAccess.Plugins;
    using NIHEI.SC4Buddy.Entities;
    using NIHEI.SC4Buddy.Localization;
    using NIHEI.SC4Buddy.View.Elements;

    public partial class FolderScannerForm : Form
    {
        public const int ErrorIconPadding = -18;

        private readonly PluginFileRegistry pluginFileRegistry;

        private readonly PluginRegistry pluginRegistry;

        private readonly PluginGroupRegistry pluginGroupRegistry;

        public FolderScannerForm(UserFolder userFolder)
        {
            pluginFileRegistry = RegistryFactory.PluginFileRegistry;

            pluginRegistry = RegistryFactory.PluginRegistry;

            pluginGroupRegistry = RegistryFactory.PluginGroupRegistry;

            UserFolder = userFolder;

            InitializeComponent();
        }

        public UserFolder UserFolder { get; private set; }

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
            foreach (ColumnHeader column in newFilesListView.Columns)
            {
                column.Width = -2;
            }

            foreach (ColumnHeader column in pluginFilesListView.Columns)
            {
                column.Width = -2;
            }
        }

        private void RescanButtonClick(object sender, EventArgs e)
        {
            scanningProgressLabel.Visible = true;
            Update();
            var folderScanner = new FolderScanner(pluginFileRegistry, UserFolder);

            if (!folderScanner.ScanFolder())
            {
                MessageBox.Show(
                    this,
                    LocalizationStrings.NoNewDeletedOrUpdatedFilesDetected,
                    LocalizationStrings.NoFileChangesDetected,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1);
                Close();
            }

            var newFiles = folderScanner.NewFiles.ToList();

            newFilesListView.BeginUpdate();
            newFilesListView.Items.Clear();

            foreach (var file in newFiles)
            {
                var filename = file.Remove(0, UserFolder.PluginFolderPath.Length + 1);
                newFilesListView.Items.Add(filename);
            }

            ResizeColumns();

            newFilesListView.EndUpdate();

            if (newFiles.Any())
            {
                addAllButton.Enabled = true;
            }

            scanningProgressLabel.Visible = false;
        }

        private void SaveButtonClick(object sender, EventArgs e)
        {
            if (!ValidatePluginInfo())
            {
                return;
            }

            var plugin = new Plugin
                             {
                                 Name = nameTextBox.Text.Trim(),
                                 Author = authorTextBox.Text.Trim(),
                                 Link = linkTextBox.Text.Trim(),
                                 Group = GetSelectedGroup(),
                                 Description = descriptionTextBox.Text.Trim(),
                                 UserFolder = UserFolder
                             };

            pluginRegistry.Add(plugin);

            var group = plugin.Group;
            if (group != null)
            {
                group.Plugins.Add(plugin);
                pluginGroupRegistry.Update(group);
            }

            pluginFileRegistry.BeginUpdate();
            foreach (var pluginFile in
                pluginFilesListView.Items.Cast<ListViewItem>()
                                   .Select(item => item.Text)
                                   .Select(
                                       path =>
                                       new PluginFile
                                           {
                                               Path = Path.Combine(UserFolder.PluginFolderPath, path),
                                               Checksum = Md5ChecksumUtility.CalculateChecksum(Path.Combine(UserFolder.PluginFolderPath, path)).ToHex(),
                                               Plugin = plugin
                                           }))
            {
                pluginFileRegistry.Add(pluginFile);
                plugin.Files.Add(pluginFile);
            }

            pluginFileRegistry.EndUpdate();

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
                pluginGroupRegistry.PluginGroups.FirstOrDefault(
                    x => x.Name.Equals(selectedText, StringComparison.OrdinalIgnoreCase));

            if (group == null)
            {
                group = new PluginGroup { Name = selectedText.Trim() };
                pluginGroupRegistry.Add(group);
            }

            return group;
        }

        private void FolderScannerFormLoad(object sender, EventArgs e)
        {
            groupComboBox.BeginUpdate();

            foreach (var pluginGroup in pluginGroupRegistry.PluginGroups)
            {
                groupComboBox.Items.Add(new ComboBoxItem<PluginGroup>(pluginGroup.Name, pluginGroup));
            }

            groupComboBox.EndUpdate();
        }

        private void NameTextBoxTextChanged(object sender, EventArgs e)
        {
            ValidatePluginInfo(false);
        }
    }
}
