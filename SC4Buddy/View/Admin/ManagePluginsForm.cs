namespace NIHEI.SC4Buddy.View.Admin
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
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

        private ICollection<RemotePluginFile> pluginFiles;

        private ICollection<RemotePlugin> pluginDependencies;

        public ManagePluginsForm(RemotePluginController remotePluginController, AuthorController authorController)
        {
            this.remotePluginController = remotePluginController;
            this.authorController = authorController;

            InitializeComponent();
        }

        private void SearchComboBoxValueChanged(object sender, EventArgs e)
        {
            var text = searchComboBox.Text.Trim().ToUpper();

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

            pluginFiles = new Collection<RemotePluginFile>();
            pluginDependencies = new Collection<RemotePlugin>();

            filesButton.Text = LocalizationStrings.Files;
            dependenciesButton.Text = LocalizationStrings.Dependencies;
        }

        private void AuthorComboBoxTextUpdated(object sender, EventArgs e)
        {
            var text = authorComboBox.Text.Trim();

            if (text.Count() < 2)
            {
                return;
            }

            var authors = authorController.Authors.Where(x => x.Name.ToUpper().Contains(text));

            var source = new AutoCompleteStringCollection();
            source.AddRange(authors.Select(x => x.Name).ToArray());
        }

        private void FilesButtonClick(object sender, EventArgs e)
        {
            using (var dialog = new ManagePluginFilesForm(pluginFiles))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    pluginFiles = dialog.PluginFiles;
                    filesButton.Text = string.Format("{0} ({1})", LocalizationStrings.Files, pluginFiles.Count);
                }
            }
        }

        private void AddButtonClick(object sender, EventArgs e)
        {
            ClearAddPluginFieldsAndVariables();
            SetAddPluginFieldsAndButtonsEnabledState(true);

            addButton.Enabled = false;
            importButton.Enabled = false;
            searchComboBox.Enabled = false;
            searchComboBox.Text = string.Empty;

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

            searchComboBox.Enabled = true;

            importButton.Enabled = true;
            removeButton.Enabled = false;
            updateButton.Enabled = false;
        }

        private void DependenciesButtonClick(object sender, EventArgs e)
        {
            var dialog = new ManagePluginDependenciesForm(pluginDependencies, remotePluginController, authorController);
            if (dialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            pluginDependencies = dialog.Dependencies;
            dependenciesButton.Text = string.Format("{0} ({1})", LocalizationStrings.Dependencies, pluginDependencies.Count);
        }

        private void SaveDataButtonClick(object sender, EventArgs e)
        {
            var plugin = new RemotePlugin
                             {
                                 Name = nameTextBox.Text.Trim(),
                                 Link = linkTextBox.Text.Trim(),
                                 Description = descriptionTextBox.Text.Trim(),
                                 Author = authorController.GetAuthorByName(authorComboBox.Text.Trim())
                             };

            foreach (var file in pluginFiles)
            {
                plugin.PluginFiles.Add(file);
            }

            foreach (var dependency in pluginDependencies)
            {
                plugin.Dependencies.Add(dependency);
            }

            remotePluginController.Add(plugin);

        }
    }
}
