namespace NIHEI.SC4Buddy.View.Plugins
{
    using System;
    using System.Linq;
    using System.Security.Policy;
    using System.Windows.Forms;

    using NIHEI.SC4Buddy.Control.Plugins;
    using NIHEI.SC4Buddy.Model;
    using NIHEI.SC4Buddy.View.Elements;

    public partial class EnterPluginInformationForm : Form
    {
        private readonly PluginGroupController pluginGroupController;

        private Plugin plugin;

        public EnterPluginInformationForm(PluginGroupController pluginGroupController)
        {
            InitializeComponent();

            this.pluginGroupController = pluginGroupController;
        }

        public Plugin Plugin
        {
            private get
            {
                return plugin;
            }

            set
            {
                plugin = value;
                ClearForm();
                nameTextBox.Text = plugin.Name;
                pluginNameLabel.Text = plugin.Name;
                authorTextBox.Text = plugin.Author;
                descriptionTextBox.Text = plugin.Description;

                if (plugin.Link != null)
                {
                    linkTextBox.Text = plugin.Link.Value;
                }

                if (plugin.PluginGroup != null)
                {
                    groupComboBox.Text = plugin.PluginGroup.Name;
                }

                installedFilesListView.BeginUpdate();

                installedFilesListView.Items.Clear();
                foreach (var relativePath in value.PluginFiles
                    .Where(x => x.QuarantinedFile == null)
                    .Select(file => file.Path.Substring(plugin.UserFolder.PluginFolderPath.Length + 1)))
                {
                    installedFilesListView.Items.Add(relativePath);
                }

                installedFilesListView.Columns[0].Width = -2;
                installedFilesListView.EndUpdate();
            }
        }

        private void ClearForm()
        {
            nameTextBox.Text = string.Empty;
            descriptionTextBox.Text = string.Empty;
            authorTextBox.Text = string.Empty;
            linkTextBox.Text = string.Empty;
            groupComboBox.SelectedIndex = -1;
        }

        private void CancelButtonClick(object sender, EventArgs e)
        {
            Close();
        }

        private void OkButtonClick(object sender, EventArgs e)
        {
            var oldGroup = plugin.PluginGroup;
            Plugin.Name = nameTextBox.Text.Trim();
            Plugin.Author = authorTextBox.Text.Trim();
            Plugin.Description = descriptionTextBox.Text.Trim();

            if (!string.IsNullOrWhiteSpace(linkTextBox.Text))
            {
                Plugin.Link = new Url(linkTextBox.Text.Trim());
            }

            Plugin.PluginGroup = GetOrCreateGroup();

            if (oldGroup != null && !oldGroup.Equals(Plugin.PluginGroup))
            {
                oldGroup.Plugins.Remove(plugin);
                pluginGroupController.SaveChanges();
            }

            if (plugin.PluginGroup != null)
            {
                plugin.PluginGroup.Plugins.Add(plugin);
                pluginGroupController.SaveChanges();
            }

            Close();
        }

        private PluginGroup GetOrCreateGroup()
        {
            if (groupComboBox.Text.Trim().Length <= 0)
            {
                return null;
            }

            var foundGroup =
                pluginGroupController.Groups.FirstOrDefault(
                    x => x.Name.Equals(groupComboBox.Text.Trim(), StringComparison.OrdinalIgnoreCase));
            if (foundGroup != null)
            {
                return foundGroup;
            }

            var newGroup = new PluginGroup { Name = groupComboBox.Text };

            pluginGroupController.Add(newGroup);
            return newGroup;
        }

        private void EnterPluginInformationFormLoad(object sender, EventArgs e)
        {
            var groups = pluginGroupController.Groups;

            foreach (var pluginGroup in groups)
            {
                groupComboBox.Items.Add(new ComboBoxItem<PluginGroup>(pluginGroup.Name, pluginGroup));
                groupComboBox.AutoCompleteCustomSource.Add(pluginGroup.Name);
            }
        }
    }
}
