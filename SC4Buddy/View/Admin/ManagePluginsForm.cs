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
    using NIHEI.SC4Buddy.View.Admin.ManagePlugins;

    public partial class ManagePluginsForm : Form
    {
        private readonly RemotePluginController remotePluginController;

        private readonly AuthorController authorController;

        private RemotePlugin selectedPlugin;

        private ICollection<RemotePluginFile> pluginFiles;

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

            if (plugin == null)
            {
                addButton.Enabled = false;
                removeButton.Enabled = false;
                updateButton.Enabled = false;
                importButton.Enabled = true;

                filesButton.Enabled = false;
                dependenciesButton.Enabled = false;

                nameTextBox.Text = string.Empty;
                nameTextBox.Enabled = false;

                authorComboBox.Text = string.Empty;
                authorComboBox.Enabled = false;

                linkTextBox.Text = string.Empty;
                linkTextBox.Enabled = false;

                descriptionTextBox.Text = string.Empty;
                descriptionTextBox.Enabled = false;

                return;
            }

            selectedPlugin = plugin;

            nameTextBox.Text = plugin.Name;
            authorComboBox.Text = plugin.Author.Name;
            linkTextBox.Text = plugin.Link;
            descriptionTextBox.Text = plugin.Description;

            nameTextBox.Enabled = false;
            authorComboBox.Enabled = false;
            linkTextBox.Enabled = false;
            descriptionTextBox.Enabled = false;

            addButton.Enabled = false;
            removeButton.Enabled = true;
            updateButton.Enabled = true;
            importButton.Enabled = false;

            filesButton.Enabled = true;
            dependenciesButton.Enabled = true;
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
                    filesButton.Text = string.Format("Files ({0})", pluginFiles.Count);
                }
            }
        }

        private void AddButtonClick(object sender, EventArgs e)
        {
            nameTextBox.Enabled = true;
            nameTextBox.Text = string.Empty;
            authorComboBox.Enabled = true;
            authorComboBox.Text = string.Empty;
            linkTextBox.Enabled = true;
            linkTextBox.Text = string.Empty;
            descriptionTextBox.Enabled = true;
            descriptionTextBox.Text = string.Empty;
            cancelDataButton.Enabled = true;

            addButton.Enabled = false;
            importButton.Enabled = false;
            searchComboBox.Enabled = false;
            searchComboBox.Text = string.Empty;


            pluginsListView.Items.Add(new ListViewItemWithObjectValue<RemotePlugin>(selectedPlugin.Name, selectedPlugin));
            nameTextBox.Focus();

            selectedPlugin = new RemotePlugin();

            pluginFiles = new Collection<RemotePluginFile>();
        }

        private void CancelDataButtonClick(object sender, EventArgs e)
        {
            selectedPlugin = null;
            pluginFiles = null;

            saveDataButton.Enabled = false;
            cancelDataButton.Enabled = false;

            addButton.Enabled = true;

            pluginsListView.Items.Remove(new ListViewItemWithObjectValue<RemotePlugin>(selectedPlugin.Name, selectedPlugin));

            nameTextBox.Enabled = false;
            nameTextBox.Text = string.Empty;
            authorComboBox.Enabled = false;
            authorComboBox.Text = string.Empty;
            linkTextBox.Enabled = false;
            linkTextBox.Text = string.Empty;
            descriptionTextBox.Enabled = false;
            descriptionTextBox.Text = string.Empty;

            filesButton.Enabled = false;
            filesButton.Text = "Files";
            dependenciesButton.Enabled = false;
            dependenciesButton.Text = "Dependencies";

            searchComboBox.Enabled = true;

            importButton.Enabled = true;
            removeButton.Enabled = false;
            updateButton.Enabled = false;
        }
    }
}
