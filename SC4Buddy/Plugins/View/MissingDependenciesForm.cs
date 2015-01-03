using System;
using System.Windows.Forms;

namespace NIHEI.SC4Buddy.View.Plugins
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using NIHEI.Common.UI.Elements;
    using NIHEI.SC4Buddy.Localization;

    using RemotePlugin = Irradiated.Sc4Buddy.ApiClient.Model.Plugin;
    using RemotePluginFile = Irradiated.Sc4Buddy.ApiClient.Model.PluginFile;

    public partial class MissingDependenciesForm : Form
    {
        private IEnumerable<RemotePlugin> missingDependencies;

        private RemotePlugin selectedItem;

        private IList<RemotePlugin> visitedDependencies;

        public MissingDependenciesForm()
        {
            InitializeComponent();
        }

        public IEnumerable<RemotePlugin> MissingDependencies
        {
            set
            {
                missingDependencies = value;
                visitedDependencies = new List<RemotePlugin>();

                UpdateListView();
            }
            get
            {
                return missingDependencies;
            }
        }

        private void UpdateListView()
        {
            var dependencies = ClearDuplicatesFromDependencies(MissingDependencies);

            dependencyListView.BeginUpdate();
            dependencyListView.Items.Clear();

            foreach (var remotePlugin in dependencies)
            {
                var item = new ListViewItemWithObjectValue<RemotePlugin>(remotePlugin.Name, remotePlugin);
                item.SubItems.Add(remotePlugin.AuthorName);
                item.SubItems.Add(remotePlugin.LinkToDownloadPage);
                item.SubItems.Add(visitedDependencies.Contains(remotePlugin) ? LocalizationStrings.Visited : string.Empty);
                dependencyListView.Items.Add(item);
            }
            dependencyListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            dependencyListView.EndUpdate();
        }

        private IEnumerable<RemotePlugin> ClearDuplicatesFromDependencies(IEnumerable<RemotePlugin> remotePlugins)
        {
            return remotePlugins.Distinct(new RemotePluginComparer());
        }

        private void DependencyListViewSelectedIndexChanged(object sender, EventArgs e)
        {
            if (dependencyListView.SelectedItems.Count > 0)
            {
                var item =
                    ((ListViewItemWithObjectValue<RemotePlugin>)dependencyListView.SelectedItems[0])
                        .Value;

                selectedItem = item;

                goToDownloadButton.Enabled = true;
            }
            else
            {
                goToDownloadButton.Enabled = false;
            }
        }

        private void GoToDownloadButtonClick(object sender, EventArgs e)
        {
            Process.Start(selectedItem.LinkToDownloadPage);
            if (!visitedDependencies.Contains(selectedItem))
            {
                visitedDependencies.Add(selectedItem);
            }
            UpdateListView();
        }
    }
}
