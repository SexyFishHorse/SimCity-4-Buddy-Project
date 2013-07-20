namespace NIHEI.SC4Buddy.View.Author
{
    using System;
    using System.Linq;
    using System.Windows.Forms;

    using NIHEI.Common.UI.Elements;
    using NIHEI.SC4Buddy.DataAccess.Remote;
    using NIHEI.SC4Buddy.Entities.Remote;

    public partial class DependenciesForm : Form
    {
        private readonly RemotePluginRegistry remotePluginRegistry;

        public DependenciesForm()
        {
            remotePluginRegistry = RemoteRegistryFactory.RemotePluginRegistry;

            InitializeComponent();
        }

        private void SearchBoxTextChanged(object sender, EventArgs e)
        {
            var text = searchBox.Text.Trim().ToUpper().Replace("//WWW.", "//");

            if (text.Length < 3)
            {
                return;
            }

            var plugins =
                remotePluginRegistry.RemotePlugins.Where(x
                    => x.Name.ToUpper().Contains(text)
                    || x.Author.Name.ToUpper().Contains(text)
                    || x.Link.ToUpper().Replace("//WWW.", "//").Contains(text));

            searchResultListView.BeginUpdate();
            searchResultListView.Items.Clear();
            foreach (var remotePlugin in plugins)
            {
                var item = new ListViewItemWithObjectValue<RemotePlugin>(remotePlugin.Name, remotePlugin);
                item.SubItems.Add(remotePlugin.Author.Name);
                item.SubItems.Add(remotePlugin.Link);
            }

            searchResultListView.EndUpdate();
            searchResultListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }
    }
}
