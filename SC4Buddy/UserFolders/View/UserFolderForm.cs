namespace Nihei.SC4Buddy.UserFolders.View
{
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;
    using Nihei.SC4Buddy.Model;
    using Nihei.SC4Buddy.Plugins.Control;
    using Nihei.SC4Buddy.Plugins.Services;
    using Nihei.SC4Buddy.Plugins.View;
    using Nihei.SC4Buddy.UserFolders.Control;

    public partial class UserFolderForm : Form
    {
        private readonly IPluginsController pluginsController;

        private readonly PluginGroupController pluginGroupController;

        private readonly IUserFoldersController userFoldersController;

        private readonly IPluginMatcher pluginMatcher;

        private PluginsForm pluginsForm;

        public UserFolderForm(
            UserFolder userFolder,
            PluginGroupController pluginGroupController,
            IUserFoldersController userFoldersController,
            IPluginMatcher pluginMatcher,
            IPluginsController pluginsController)
        {
            UserFolder = userFolder;
            this.pluginGroupController = pluginGroupController;
            this.userFoldersController = userFoldersController;
            this.pluginMatcher = pluginMatcher;
            this.pluginsController = pluginsController;
            InitializeComponent();
        }

        public UserFolder UserFolder { get; set; }

        public void UpdateData()
        {
            Text = UserFolder.Alias;
            numberOfPluginsLabel.Text = pluginsController.Plugins.Count.ToString(CultureInfo.InvariantCulture);

            var directoryInfo = new DirectoryInfo(UserFolder.PluginFolderPath);
            directoryInfo.Create();
            var files = directoryInfo.EnumerateFiles("*", SearchOption.AllDirectories);
            var size = files.Sum(fileInfo => fileInfo.Length);

            sizeOfPluginsLabel.Text = ByteSize.ByteSize.FromBytes(size).ToString("#.##");
        }

        private void ManagePluginsButtonClick(object sender, System.EventArgs e)
        {
            Hide();

            if (pluginsForm == null)
            {
                pluginsForm = new PluginsForm(
                    pluginGroupController,
                    userFoldersController,
                    pluginsController,
                    UserFolder,
                    pluginMatcher);
            }

            pluginsForm.ReloadAndRepopulate();
            pluginsForm.Show(this);
        }

        private void UserFolderFormLoad(object sender, System.EventArgs e)
        {
            UpdateData();
        }

        private void UserFolderFormFormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}
