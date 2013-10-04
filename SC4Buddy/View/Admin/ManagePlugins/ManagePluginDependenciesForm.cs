namespace NIHEI.SC4Buddy.View.Admin.ManagePlugins
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows.Forms;

    using NIHEI.Common.UI.Elements;
    using NIHEI.SC4Buddy.Entities.Remote;

    public partial class ManagePluginDependenciesForm : Form
    {
        public ManagePluginDependenciesForm(Collection<RemotePlugin> pluginDependencies)
        {
            Dependencies = pluginDependencies;
            InitializeComponent();

            UpdateDependencyListView(Dependencies);
        }

        public Collection<RemotePlugin> Dependencies { get; private set; }

        private void UpdateDependencyListView(IEnumerable<RemotePlugin> dependencies)
        {
            dependenciesListView.BeginUpdate();
            dependenciesListView.Items.Clear();

            foreach (var dependency in dependencies)
            {
                var item = new ListViewItemWithObjectValue<RemotePlugin>(dependency.Name, dependency);
                item.SubItems.Add(dependency.Author.Name);

                dependenciesListView.Items.Add(item);
            }

            dependenciesListView.EndUpdate();
        }
    }
}
