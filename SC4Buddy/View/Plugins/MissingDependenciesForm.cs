using System;
using System.Windows.Forms;

namespace NIHEI.SC4Buddy.View.Plugins
{
    using System.Collections.Generic;
    using System.Diagnostics;

    using NIHEI.Common.UI.Elements;
    using NIHEI.SC4Buddy.Entities.Remote;
    using NIHEI.SC4Buddy.Localization;

    public partial class MissingDependenciesForm : Form
    {
        private IEnumerable<RemotePlugin> missingDependencies;
        private string link;

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
        }

        private void DependencyListViewSelectedIndexChanged(object sender, EventArgs e)
        {
            if (dependencyListView.SelectedItems.Count > 0)
            {
                var item =
                    ((ListViewItemWithObjectValue<Entities.Remote.RemotePlugin>)dependencyListView.SelectedItems[0])
                        .Value;

                link = item.Link;

                goToDownloadButton.Enabled = true;
            }
            else
            {
                goToDownloadButton.Enabled = false;
            }
        }

        private void GoToDownloadButtonClick(object sender, EventArgs e)
        {
            Process.Start(link);
        }
    }
}
