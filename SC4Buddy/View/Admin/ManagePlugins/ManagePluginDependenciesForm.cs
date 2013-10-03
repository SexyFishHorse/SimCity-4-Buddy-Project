namespace NIHEI.SC4Buddy.View.Admin.ManagePlugins
{
    using System.Collections.ObjectModel;
    using System.Windows.Forms;

    using NIHEI.SC4Buddy.Entities.Remote;

    public partial class ManagePluginDependenciesForm : Form
    {
        public ManagePluginDependenciesForm(Collection<RemotePlugin> pluginDependencies)
        {
            Dependencies = pluginDependencies;
            InitializeComponent();
        }

        public Collection<RemotePlugin> Dependencies { get; private set; }
    }
}
