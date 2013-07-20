namespace NIHEI.SC4Buddy.View.Author
{
    using System;
    using System.Linq;
    using System.Windows.Forms;

    using NIHEI.SC4Buddy.DataAccess;
    using NIHEI.SC4Buddy.DataAccess.Plugins;
    using NIHEI.SC4Buddy.Entities;
    using NIHEI.SC4Buddy.View.Elements;

    public partial class SelectInstalledPluginForm : Form
    {
        private readonly UserFolderRegistry userFolderRegistry;

        private readonly PluginRegistry pluginRegistry;

        public SelectInstalledPluginForm()
        {
            userFolderRegistry = RegistryFactory.UserFolderRegistry;

            pluginRegistry = RegistryFactory.PluginRegistry;

            InitializeComponent();
        }

        public Plugin SelectedPlugin { get; set; }

        private void SelectInstalledPluginFormLoad(object sender, EventArgs e)
        {
            var userFolders = userFolderRegistry.UserFolders.Where(x => x.Plugin.Any());

            foreach (var userFolder in userFolders)
            {
                userFolderComboBox.Items.Add(new ComboBoxItem<UserFolder>(userFolder.Alias, userFolder));
            }

            pluginComboBox.Enabled = false;
        }

        private void UserFolderComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            if (userFolderComboBox.SelectedItem != null)
            {
                pluginComboBox.Enabled = true;

                var userFolder = ((ComboBoxItem<UserFolder>)userFolderComboBox.SelectedItem).Value;
                var plugins = pluginRegistry.Plugins.Where(x => x.UserFolderId == userFolder.Id);

                foreach (var plugin in plugins)
                {
                    pluginComboBox.Items.Add(new ComboBoxItem<Plugin>(plugin.Name, plugin));
                }
            }
            else
            {
                pluginComboBox.Enabled = false;
            }
        }

        private void PluginComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            var plugin = ((ComboBoxItem<Plugin>)pluginComboBox.SelectedItem).Value;

            if (plugin != null)
            {
                okButton.Enabled = true;

                SelectedPlugin = plugin;
            }
            else
            {
                okButton.Enabled = false;
            }
        }
    }
}
