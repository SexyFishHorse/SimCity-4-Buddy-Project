namespace NIHEI.SC4Buddy.View.Plugins
{
    using System;
    using System.Linq;
    using System.Windows.Forms;

    using NIHEI.SC4Buddy.Control.Plugins;
    using NIHEI.SC4Buddy.Entities;
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
                linkTextBox.Text = plugin.Link;
                descriptionTextBox.Text = plugin.Description;

                if (plugin.Group != null)
                {
                    groupComboBox.Text = plugin.Group.Name;
                }

                installedFilesListView.BeginUpdate();

                installedFilesListView.Items.Clear();
                foreach (var relativePath in value.Files
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
            var oldGroup = plugin.Group;
            Plugin.Name = nameTextBox.Text.Trim();
            Plugin.Author = authorTextBox.Text.Trim();
            Plugin.Description = descriptionTextBox.Text.Trim();
            Plugin.Link = linkTextBox.Text.Trim();
            Plugin.Group = GetOrCreateGroup();

            if (oldGroup != null && !oldGroup.Equals(Plugin.Group))
            {
                oldGroup.Plugins.Remove(plugin);
                pluginGroupController.SaveChanges();
            }

            if (plugin.Group != null)
            {
                plugin.Group.Plugins.Add(plugin);
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
