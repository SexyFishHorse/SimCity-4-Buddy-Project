namespace NIHEI.SC4Buddy.View.Plugins
{
    using System;
    using System.Linq;
    using System.Windows.Forms;

    using NIHEI.SC4Buddy.DataAccess;
    using NIHEI.SC4Buddy.DataAccess.Plugins;
    using NIHEI.SC4Buddy.Entities;
    using NIHEI.SC4Buddy.View.Elements;

    public partial class EnterPluginInformationForm : Form
    {
        private readonly PluginGroupRegistry pluginGroupRegistry;

        private Plugin plugin;

        public EnterPluginInformationForm()
        {
            InitializeComponent();

            pluginGroupRegistry = RegistryFactory.PluginGroupRegistry;
        }

        public Plugin Plugin
        {
            get
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
                foreach (var file in value.Files)
                {
                    installedFilesListView.Items.Add(file.Path);
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

            if (oldGroup != null)
            {
                oldGroup.Plugins.Remove(plugin);
                pluginGroupRegistry.Update(oldGroup);
            }

            if (plugin.Group != null)
            {
                plugin.Group.Plugins.Add(plugin);
                pluginGroupRegistry.Update(plugin.Group);
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
                pluginGroupRegistry.PluginGroups.FirstOrDefault(
                    x => x.Name.Equals(groupComboBox.Text.Trim(), StringComparison.OrdinalIgnoreCase));
            if (foundGroup != null)
            {
                return foundGroup;
            }

            var newGroup = new PluginGroup { Name = groupComboBox.Text };

            pluginGroupRegistry.Add(newGroup);
            return newGroup;
        }

        private void EnterPluginInformationFormLoad(object sender, EventArgs e)
        {
            var groups = pluginGroupRegistry.PluginGroups;

            foreach (var pluginGroup in groups)
            {
                groupComboBox.Items.Add(new ComboBoxItem<PluginGroup>(pluginGroup.Name, pluginGroup));
                groupComboBox.AutoCompleteCustomSource.Add(pluginGroup.Name);
            }
        }
    }
}
