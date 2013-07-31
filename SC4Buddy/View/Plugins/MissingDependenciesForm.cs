using System;
using System.Windows.Forms;

namespace NIHEI.SC4Buddy.View.Plugins
{
    using NIHEI.Common.UI.Elements;

    public partial class MissingDependenciesForm : Form
    {
        private string link;

        public MissingDependenciesForm()
        {
            InitializeComponent();
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
    }
}
