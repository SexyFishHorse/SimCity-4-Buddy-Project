namespace NIHEI.SC4Buddy.View.Admin.ManagePlugins
{
    using System.Data.Objects.DataClasses;
    using System.Windows.Forms;

    using NIHEI.SC4Buddy.Entities.Remote;

    public partial class ManagePluginFilesForm : Form
    {
        public ManagePluginFilesForm(EntityCollection<RemotePluginFile> pluginFiles)
        {
            PluginFiles = pluginFiles;
            InitializeComponent();
        }

        public EntityCollection<RemotePluginFile> PluginFiles { get; private set; }
    }
}
