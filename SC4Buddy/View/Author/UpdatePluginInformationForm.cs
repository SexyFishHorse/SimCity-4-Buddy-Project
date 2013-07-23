namespace NIHEI.SC4Buddy.View.Author
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;

    using NIHEI.SC4Buddy.Control;
    using NIHEI.SC4Buddy.DataAccess.Remote;
    using NIHEI.SC4Buddy.Entities.Remote;
    using NIHEI.SC4Buddy.View.Elements;

    public partial class UpdatePluginInformationForm : Form
    {
        private readonly RemotePluginRegistry remotePluginRegistry;

        private readonly AuthorRegistry authorRegistry;

        private IList<RemotePlugin> dependencies;

        private IList<RemotePluginFile> files;

        public UpdatePluginInformationForm()
        {
            remotePluginRegistry = RemoteRegistryFactory.RemotePluginRegistry;
            authorRegistry = RemoteRegistryFactory.AuthorRegistry;

            dependencies = new List<RemotePlugin>();
            files = new List<RemotePluginFile>();

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
                searchResultsListView.Items.Add(item);
            }

            searchResultsListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
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
                        || x.Link.ToUpper().Replace("//WWW.", "//").Contains(text))).ToList();

            UpdateSearchResultListView(plugins);
        }

        private void UpdatePluginInformationFormLoad(object sender, EventArgs e)
        {
            var authors = authorRegistry.Authors.Where(x => x.UserId == SessionController.Instance.User.Id);

            siteAndAuthorComboBox.BeginUpdate();
            foreach (var author in authors)
            {
                siteAndAuthorComboBox.Items.Add(new ComboBoxItem<Author>(string.Format("{0} ({1})", author.Site, author.Name), author));
            }

            siteAndAuthorComboBox.EndUpdate();
        }

        private void SearchResultsListViewSelectedIndexChanged(object sender, EventArgs e)
        {
            if (searchResultsListView.SelectedItems.Count < 1)
            {
                return;
            }

            var item = ((ListViewItemWithObjectValue<RemotePlugin>)searchResultsListView.SelectedItems[0]).Value;

            nameTextBox.Enabled = true;
            nameTextBox.Text = item.Name;
            linkTextBox.Enabled = true;
            linkTextBox.Text = item.Link;
            descriptionTextBox.Enabled = true;
            descriptionTextBox.Text = item.Description;
            SelectAuthorInList(item.Author.Name, item.Author.Site);
            siteAndAuthorComboBox.Enabled = true;

            dependencies = item.Dependencies.ToList();
            dependenciesButton.Enabled = true;
            files = item.Files.ToList();
            filesButton.Enabled = true;

            saveButton.Enabled = true;
            deleteButton.Enabled = true;
        }

        private void SelectAuthorInList(string author, string link)
        {
            var authors = siteAndAuthorComboBox.Items;

            foreach (var comboBoxItem in
                authors.Cast<ComboBoxItem<Author>>()
                       .Where(
                           comboBoxItem =>
                           link.Replace("//www.", "//")
                               .ToUpper()
                               .Contains(comboBoxItem.Value.Site.Replace("//www.", "//").ToUpper())
                           && comboBoxItem.Value.Name.ToUpper().Equals(author.ToUpper())))
            {
                siteAndAuthorComboBox.SelectedItem = comboBoxItem;
            }
        }

        private void FilesButtonClick(object sender, EventArgs e)
        {
            var dialog = new FilesForm(files);

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                files = dialog.Files;
            }
        }

        private void DependenciesButtonClick(object sender, EventArgs e)
        {
            var dialog = new DependenciesForm(dependencies);

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                dependencies = dialog.Dependencies.ToList();
            }
        }
    }
}
