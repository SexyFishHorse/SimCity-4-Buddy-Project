namespace NIHEI.SC4Buddy.View.Author
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;

    using NIHEI.Common.UI.Elements;
    using NIHEI.SC4Buddy.Control;
    using NIHEI.SC4Buddy.DataAccess.Remote;
    using NIHEI.SC4Buddy.Entities.Remote;

    public partial class UpdatePluginInformationForm : Form
    {
        private readonly RemotePluginRegistry remotePluginRegistry;

        public UpdatePluginInformationForm()
        {
            remotePluginRegistry = RemoteRegistryFactory.RemotePluginRegistry;

            InitializeComponent();
        }

        private void UpdateSearchResultListView(IEnumerable<RemotePlugin> plugins)
        {
            searchResultsListView.BeginUpdate();
            searchResultsListView.Items.Clear();

            foreach (var plugin in plugins)
            {
                var item = new ListViewItemWithObjectValue<RemotePlugin>(plugin.Name, plugin);
                item.SubItems.Add(plugin.Author.Name);
                item.SubItems.Add(plugin.Link);
            }

            searchResultsListView.EndUpdate();
        }

        private void SearchTextBoxTextChanged(object sender, EventArgs e)
        {
            var text = searchTextBox.Text.Trim().ToUpper().Replace("//WWW.", "//");

            if (text.Length < 3)
            {
                searchResultsListView.Items.Clear();
                return;
            }

            var plugins =
                remotePluginRegistry.RemotePlugins.Include("Author").Where(
                    x =>
                    x.Author != null && x.Author.User != null && x.Author.UserId == SessionController.Instance.User.Id
                    && (x.Name.ToUpper().Contains(text) || x.Author.Name.ToUpper().Contains(text)
                        || x.Link.ToUpper().Replace("//WWW.", "//").Contains(text)));

            UpdateSearchResultListView(plugins);
        }
    }
}
