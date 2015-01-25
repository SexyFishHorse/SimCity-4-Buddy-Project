namespace NIHEI.SC4Buddy.UserFolders.View
{
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;
    using NIHEI.SC4Buddy.Plugins.Control;
    using NIHEI.SC4Buddy.Plugins.View;
    using NIHEI.SC4Buddy.Remote;
    using NIHEI.SC4Buddy.UserFolders.Control;

    public partial class UserFolderForm : Form
    {
        private readonly PluginController pluginController;

        private readonly PluginGroupController pluginGroupController;

        private readonly IUserFolderController userFolderController;

        private readonly IPluginMatcher pluginMatcher;

        private readonly IDependencyChecker dependencyChecker;

        private readonly IUserFolderRepository userFolderRepository;

        public UserFolderForm(
            PluginController pluginController,
            PluginGroupController pluginGroupController,
            IUserFolderController userFolderController,
            IPluginMatcher pluginMatcher,
            IDependencyChecker dependencyChecker, 
            IUserFolderRepository userFolderRepository)
        {
            this.pluginController = pluginController;
            this.pluginGroupController = pluginGroupController;
            this.userFolderController = userFolderController;
            this.pluginMatcher = pluginMatcher;
            this.dependencyChecker = dependencyChecker;
            this.userFolderRepository = userFolderRepository;
            InitializeComponent();
        }

        private void ManagePluginsButtonClick(object sender, System.EventArgs e)
        {
            var dialog = new PluginsForm(
                pluginController,
                pluginGroupController,
                userFolderController,
                pluginMatcher,
                dependencyChecker,
                userFolderRepository);
            dialog.ShowDialog(this);
        }

        private void UserFolderFormLoad(object sender, System.EventArgs e)
        {
            Text = userFolderController.UserFolder.Alias;
            numberOfPluginsLabel.Text = pluginController.Plugins.Count.ToString(CultureInfo.InvariantCulture);

            var directoryInfo = new DirectoryInfo(userFolderController.UserFolder.PluginFolderPath);
            var files = directoryInfo.EnumerateFiles("*", SearchOption.AllDirectories);
            var size = files.Sum(fileInfo => fileInfo.Length);

            sizeOfPluginsLabel.Text = ByteSize.ByteSize.FromBytes(size).ToString("#.##");
        }
    }
}
