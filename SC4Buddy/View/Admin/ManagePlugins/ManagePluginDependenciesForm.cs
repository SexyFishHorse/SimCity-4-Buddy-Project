namespace NIHEI.SC4Buddy.View.Admin.ManagePlugins
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows.Forms;

    using NIHEI.Common.UI.Elements;
    using NIHEI.SC4Buddy.Control.Remote;
    using NIHEI.SC4Buddy.Entities.Remote;

    public partial class ManagePluginDependenciesForm : Form
    {
        private readonly RemotePluginController remotePluginController;

        private RemotePlugin selectedDependency;

        public ManagePluginDependenciesForm(
            Collection<RemotePlugin> pluginDependencies,
            RemotePluginController remotePluginController)
        {
            Dependencies = pluginDependencies;
            this.remotePluginController = remotePluginController;
            InitializeComponent();

            UpdateListView(dependenciesListView, Dependencies);
        }

        public Collection<RemotePlugin> Dependencies { get; private set; }

        private void UpdateListView(ListView listView, IEnumerable<RemotePlugin> dependencies)
        {
            listView.BeginUpdate();
            listView.Items.Clear();

            foreach (var dependency in dependencies)
            {
                var item = new ListViewItemWithObjectValue<RemotePlugin>(dependency.Name, dependency);
                item.SubItems.Add(dependency.Author.Name);

                listView.Items.Add(item);
            }

            listView.EndUpdate();
        }

        private void DependenciesListViewSelectedIndexChanged(object sender, System.EventArgs e)
        {
            selectedDependency = dependenciesListView.SelectedItems.Count > 0
                                     ? ((ListViewItemWithObjectValue<RemotePlugin>)dependenciesListView.SelectedItems[0])
                                           .Value
                                     : null;

            removeDependencyButton.Enabled = selectedDependency != null;
        }

        private void RemoveDependencyButtonClick(object sender, System.EventArgs e)
        {
            dependenciesListView.Items.Remove(
                new ListViewItemWithObjectValue<RemotePlugin>(selectedDependency.Name, selectedDependency));

            selectedDependency = null;

            removeDependencyButton.Enabled = false;
        }

        private void SearchTextBoxTextChanged(object sender, System.EventArgs e)
        {
            var text = searchTextBox.Text.Trim();

            if (text.Length < 3)
            {
                return;
            }

            var results = remotePluginController.SearchForPlugin(text);

            UpdateListView(resultsListView, results);
        }
    }
}
