namespace NIHEI.SC4Buddy.View.Author
{
    using System;
    using System.Linq;
    using System.Windows.Forms;

    using NIHEI.SC4Buddy.DataAccess;
    using NIHEI.SC4Buddy.Entities;
    using NIHEI.SC4Buddy.View.Elements;

    public partial class SelectInstalledPluginForm : Form
    {
        private readonly UserFolderRegistry userFolderRegistry;

        public SelectInstalledPluginForm()
        {
            userFolderRegistry = RegistryFactory.UserFolderRegistry;

            InitializeComponent();
        }

        private void SelectInstalledPluginFormLoad(object sender, EventArgs e)
        {
            var userFolders = userFolderRegistry.UserFolders.Where(x => x.Plugin.Any());

            foreach (var userFolder in userFolders)
            {
                userFolderComboBox.Items.Add(new ComboBoxItem<UserFolder>(userFolder.Alias, userFolder));
            }
        }
    }
}
