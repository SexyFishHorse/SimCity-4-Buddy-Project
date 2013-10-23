namespace NIHEI.SC4Buddy.View.Admin
{
    using System;
    using System.Linq;
    using System.Windows.Forms;

    using NIHEI.Common.UI.Elements;
    using NIHEI.SC4Buddy.Control.Remote;
    using NIHEI.SC4Buddy.Entities.Remote;
    using NIHEI.SC4Buddy.Localization;
    using NIHEI.SC4Buddy.View.Admin.ManagePlugins;

    public partial class ManagePluginsForm : Form
    {
        private readonly RemotePluginController remotePluginController;

        private readonly AuthorController authorController;

        private RemotePlugin selectedPlugin;

        public ManagePluginsForm(RemotePluginController remotePluginController, AuthorController authorController)
        {
            this.remotePluginController = remotePluginController;
            this.authorController = authorController;

            InitializeComponent();
        }

        private void SearchTextBoxTextChanged(object sender, EventArgs e)
        {
            var text = searchTextBox.Text.Trim().ToUpper();

            if (text.Length < 3)
            {
                return;
            }

            var matches =
                remotePluginController.Plugins.Where(
                    x =>
                    x.Name.ToUpper().Contains(text) || x.Author.Name.ToUpper().Contains(text)
                    || x.Description.ToUpper().Contains(text) || x.Link.ToUpper().Contains(text));

            pluginsListView.BeginUpdate();
            pluginsListView.Items.Clear();

            foreach (var plugin in matches)
            {
                var item = new ListViewItemWithObjectValue<RemotePlugin>(plugin.Name, plugin);
                item.SubItems.Add(plugin.Author.Name);
                item.SubItems.Add(plugin.Link);

                pluginsListView.Items.Add(item);
            }

            pluginsListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            pluginsListView.EndUpdate();
        }

        private void PluginsListViewSelectedIndexChanged(object sender, EventArgs e)
        {
            var plugin = pluginsListView.SelectedItems.Count > 0
                           ? ((ListViewItemWithObjectValue<RemotePlugin>)pluginsListView.SelectedItems[0]).Value
                           : null;

            if (plugin != null && plugin.Id < 1)
            {
                return;
            }

            if (plugin == null)
            {
                addButton.Enabled = false;
                removeButton.Enabled = false;
                updateButton.Enabled = false;
                importButton.Enabled = true;

                ClearAddPluginFieldsAndVariables();

                SetAddPluginFieldsAndButtonsEnabledState(false);

                return;
            }

            selectedPlugin = plugin;

            nameTextBox.Text = plugin.Name;
            authorComboBox.Text = plugin.Author.Name;
            linkTextBox.Text = plugin.Link;
            descriptionTextBox.Text = plugin.Description;

            SetAddPluginFieldsAndButtonsEnabledState(true);

            addButton.Enabled = false;
            removeButton.Enabled = true;
            updateButton.Enabled = true;
            importButton.Enabled = false;
        }

        private void SetAddPluginFieldsAndButtonsEnabledState(bool enabled)
        {
            nameTextBox.Enabled = enabled;
            authorComboBox.Enabled = enabled;
            linkTextBox.Enabled = enabled;
            descriptionTextBox.Enabled = enabled;

            filesButton.Enabled = enabled;
            dependenciesButton.Enabled = enabled;
        }

        private void ClearAddPluginFieldsAndVariables()
        {
            nameTextBox.Text = string.Empty;
            authorComboBox.Text = string.Empty;
            linkTextBox.Text = string.Empty;
            descriptionTextBox.Text = string.Empty;

            filesButton.Text = LocalizationStrings.Files;
            dependenciesButton.Text = LocalizationStrings.Dependencies;
        }

        private void FilesButtonClick(object sender, EventArgs e)
        {
            using (var dialog = new ManagePluginFilesForm(selectedPlugin.PluginFiles))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    selectedPlugin.PluginFiles.Clear();
                    foreach (var file in dialog.PluginFiles)
                    {
                        selectedPlugin.PluginFiles.Add(file);
                    }

                    filesButton.Text = string.Format("{0} ({1})", LocalizationStrings.Files, selectedPlugin.PluginFiles.Count);
                }
            }

            ValidateAddPluginFormFieldsFilled();
        }

        private void AddButtonClick(object sender, EventArgs e)
        {
            ClearAddPluginFieldsAndVariables();
            SetAddPluginFieldsAndButtonsEnabledState(true);

            cancelDataButton.Enabled = true;

            addButton.Enabled = false;
            importButton.Enabled = false;

            searchTextBox.Enabled = false;
            searchTextBox.Text = string.Empty;

            selectedPlugin = new RemotePlugin { Name = LocalizationStrings.NewInParanthesis };

            pluginsListView.Items.Add(new ListViewItemWithObjectValue<RemotePlugin>(selectedPlugin.Name, selectedPlugin));

            nameTextBox.Focus();
        }

        private void CancelDataButtonClick(object sender, EventArgs e)
        {
            foreach (var item in
                pluginsListView.Items.Cast<ListViewItemWithObjectValue<RemotePlugin>>()
                    .Where(item => item.Value == selectedPlugin))
            {
                pluginsListView.Items.Remove(item);
                break;
            }

            selectedPlugin = null;

            saveDataButton.Enabled = false;
            cancelDataButton.Enabled = false;

            addButton.Enabled = true;

            ClearAddPluginFieldsAndVariables();
            SetAddPluginFieldsAndButtonsEnabledState(false);

            searchTextBox.Enabled = true;

            importButton.Enabled = true;
            removeButton.Enabled = false;
            updateButton.Enabled = false;
        }

        private void DependenciesButtonClick(object sender, EventArgs e)
        {
            var dialog = new ManagePluginDependenciesForm(selectedPlugin.Dependencies, remotePluginController, authorController);
            if (dialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            dependenciesButton.Text = string.Format("{0} ({1})", LocalizationStrings.Dependencies, selectedPlugin.Dependencies.Count);
        }

        private void SaveDataButtonClick(object sender, EventArgs e)
        {
            selectedPlugin.Name = nameTextBox.Text.Trim();
            selectedPlugin.Link = linkTextBox.Text.Trim();
            selectedPlugin.Description = descriptionTextBox.Text.Trim();
            selectedPlugin.Author = authorController.GetAuthorByName(authorComboBox.Text.Trim());

            if (selectedPlugin.Id < 1)
            {
                remotePluginController.Add(selectedPlugin);
            }

            ClearAddPluginFieldsAndVariables();
            SetAddPluginFieldsAndButtonsEnabledState(false);

            cancelDataButton.Enabled = false;
            saveDataButton.Enabled = false;

            addButton.Enabled = true;
        }

        private void NameTextBoxTextChanged(object sender, EventArgs e)
        {
            ValidateAddPluginFormFieldsFilled();
        }

        private void ValidateAddPluginFormFieldsFilled()
        {
            var enableSave = !(nameTextBox.Text.Trim().Length < 3);

            if (authorComboBox.Text.Trim().Length < 3)
            {
                enableSave = false;
            }

            if (linkTextBox.Text.Trim().Length < 3)
            {
                enableSave = false;
            }

            if (descriptionTextBox.Text.Trim().Length < 3)
            {
                enableSave = false;
            }

            if (selectedPlugin.PluginFiles.Count < 1)
            {
                enableSave = false;
            }

            saveDataButton.Enabled = enableSave;
        }

        private void LinkTextBoxTextChanged(object sender, EventArgs e)
        {
            ValidateAddPluginFormFieldsFilled();
        }

        private void DescriptionTextBoxTextChanged(object sender, EventArgs e)
        {
            ValidateAddPluginFormFieldsFilled();
        }

        private void AuthorComboBoxTextChanged(object sender, EventArgs e)
        {
            ValidateAddPluginFormFieldsFilled();

            var text = authorComboBox.Text.Trim().ToUpper();

            if (text.Count() < 3)
            {
                return;
            }

            var authors = authorController.Authors.Where(x => x.Name.ToUpper().Contains(text));

            var source = new AutoCompleteStringCollection();
            source.AddRange(authors.Select(x => x.Name).ToArray());

            authorComboBox.AutoCompleteCustomSource = source;
        }
    }
}
