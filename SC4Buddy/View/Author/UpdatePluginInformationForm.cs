namespace NIHEI.SC4Buddy.View.Author
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Windows.Forms;

    using NIHEI.SC4Buddy.Control;
    using NIHEI.SC4Buddy.Control.Remote;
    using NIHEI.SC4Buddy.Entities.Remote;
    using NIHEI.SC4Buddy.Localization;
    using NIHEI.SC4Buddy.View.Elements;

    public partial class UpdatePluginInformationForm : Form
    {
        private readonly RemotePluginController remotePluginController;

        private readonly AuthorController authorController;

        private IList<RemotePlugin> dependencies;

        private IList<RemotePluginFile> files;

        public UpdatePluginInformationForm(
            AuthorController authorController, RemotePluginController remotePluginController)
        {
            this.authorController = authorController;
            this.remotePluginController = remotePluginController;

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
                remotePluginController.Plugins.Include("Author").Where(
                    x =>
                    x.Author != null && x.Author.User != null && x.Author.UserId == SessionController.Instance.User.Id
                    && (x.Name.ToUpper().Contains(text) || x.Author.Name.ToUpper().Contains(text)
                        || x.Link.ToUpper().Replace("//WWW.", "//").Contains(text))).ToList();

            UpdateSearchResultListView(plugins);
        }

        private void UpdatePluginInformationFormLoad(object sender, EventArgs e)
        {
            var authors = authorController.Authors.Where(x => x.UserId == SessionController.Instance.User.Id);

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
            files = item.PluginFiles.ToList();
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

        private void DeleteButtonClick(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                this,
                LocalizationStrings.AreYouSureYouWantToDeleteThisPluginFromTheServer,
                LocalizationStrings.ConfirmDeletionOfPlugin,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2) == DialogResult.No)
            {
                return;
            }

            var listViewItem = searchResultsListView.SelectedItems[0];
            var item = ((ListViewItemWithObjectValue<RemotePlugin>)listViewItem).Value;

            searchResultsListView.Items.Remove(listViewItem);

            remotePluginController.Delete(item);

            ClearForm();
        }

        private void ClearForm()
        {
            nameTextBox.Text = string.Empty;
            nameTextBox.Enabled = false;
            linkTextBox.Text = string.Empty;
            linkTextBox.Enabled = false;
            siteAndAuthorComboBox.SelectedIndex = -1;
            siteAndAuthorComboBox.Enabled = false;
            descriptionTextBox.Text = string.Empty;
            descriptionTextBox.Enabled = false;
            filesButton.Enabled = false;
            dependenciesButton.Enabled = false;
            saveButton.Enabled = false;
            deleteButton.Enabled = false;
        }

        private void SaveButtonClick(object sender, EventArgs e)
        {
            var listViewItem = searchResultsListView.SelectedItems[0];
            var remotePlugin = ((ListViewItemWithObjectValue<RemotePlugin>)listViewItem).Value;

            var link = linkTextBox.Text.Trim();
            var author = ((ComboBoxItem<Author>)siteAndAuthorComboBox.SelectedItem).Value;

            if (!RemotePluginController.ValidateLinkAndAuthor(link, author))
            {
                MessageBox.Show(
                    this,
                    LocalizationStrings.TheLinkToTheDownloadInfoPageAndTheSiteOfTheAuthorDoesNotMatch,
                    LocalizationStrings.LinkAndAuthorSiteDoesNotMatch,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1);
                return;
            }

            var name = nameTextBox.Text.Trim();
            var description = descriptionTextBox.Text.Trim();

            searchResultsListView.Items.Remove(listViewItem);

            remotePlugin.Name = name;
            remotePlugin.Description = description;
            remotePlugin.Author = author;
            remotePlugin.Link = link;

            remotePlugin.PluginFiles.Clear();
            foreach (var file in files)
            {
                remotePlugin.PluginFiles.Add(file);
            }

            remotePlugin.Dependencies.Clear();
            foreach (var dependency in dependencies)
            {
                remotePlugin.Dependencies.Add(dependency);
            }

            remotePluginController.SaveChanges();

            var item = new ListViewItemWithObjectValue<RemotePlugin>(remotePlugin.Name, remotePlugin);
            item.SubItems.Add(remotePlugin.Author.Name);
            item.SubItems.Add(remotePlugin.Link);
            searchResultsListView.Items.Add(item);

            MessageBox.Show(
                this,
                LocalizationStrings.ThePluginHasBeenUpdatedInTheCentralDatabase,
                LocalizationStrings.PluginInformationUpdated,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1);

            ClearForm();
        }
    }
}
