namespace NIHEI.SC4Buddy.View.Author
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;

    using NIHEI.SC4Buddy.Control;
    using NIHEI.SC4Buddy.DataAccess.Remote;
    using NIHEI.SC4Buddy.Entities;
    using NIHEI.SC4Buddy.Entities.Remote;
    using NIHEI.SC4Buddy.Localization;
    using NIHEI.SC4Buddy.View.Elements;

    public partial class AddPluginInformationForm : Form
    {
        private readonly AuthorRegistry authorRegistry;

        private readonly RemotePluginRegistry remotePluginRegistry;

        private IList<RemotePluginFile> files;

        private ICollection<RemotePlugin> dependencies;

        public AddPluginInformationForm()
        {
            authorRegistry = RemoteRegistryFactory.AuthorRegistry;

            remotePluginRegistry = RemoteRegistryFactory.RemotePluginRegistry;

            files = new List<RemotePluginFile>();

            dependencies = new Collection<RemotePlugin>();

            InitializeComponent();
        }

        private void CancelButtonClick(object sender, EventArgs e)
        {
            Close();
        }

        private void AddPluginInformationFormLoad(object sender, EventArgs e)
        {
            var authors = authorRegistry.Authors.Where(x => x.UserId == SessionController.Instance.User.Id).ToList();

            if (!authors.Any())
            {
                MessageBox.Show(
                    this,
                    LocalizationStrings.YouNeedToAddAtLeastOneAuthorInOrderToAddPluginInformationToTheCentralDatabase,
                    LocalizationStrings.NoAuthorsDefined,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1);
                Close();
            }

            ReloadSiteAndAuthorComboBoxItems(authors);
        }

        private void ReloadSiteAndAuthorComboBoxItems(IEnumerable<Author> authors)
        {
            siteAndAuthorComboBox.BeginUpdate();
            siteAndAuthorComboBox.Items.Clear();
            foreach (var author in authors)
            {
                siteAndAuthorComboBox.Items.Add(
                    new ComboBoxItem<Author>(
                        string.Format("{1} ({0})", author.Name, author.Site), author));
            }

            siteAndAuthorComboBox.EndUpdate();
        }

        private void FilesButtonClick(object sender, EventArgs e)
        {
            var dialog = new FilesForm(files);

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                files = dialog.Files;
            }

            UpdateSaveButtonStatus();
        }

        private void SelectInstalledPluginButtonClick(object sender, EventArgs e)
        {
            var dialog = new SelectInstalledPluginForm();

            if (dialog.ShowDialog(this) == DialogResult.Cancel)
            {
                return;
            }

            var plugin = dialog.SelectedPlugin;
            files = ConvertPluginFilesToRemotePluginFiles(plugin.Files.ToList());

            if (dialog.IncludeInformation)
            {
                nameTextBox.Text = plugin.Name.Trim();
                linkTextBox.Text = plugin.Link;
                descriptionTextBox.Text = plugin.Description;
                if (plugin.Author != null && !string.IsNullOrWhiteSpace(plugin.Link))
                {
                    SelectAuthorInList(plugin.Author, plugin.Link);
                }
            }
            else if (nameTextBox.Text.Trim().Length < 1)
            {
                nameTextBox.Text = plugin.Name.Trim();
            }
        }

        private IList<RemotePluginFile> ConvertPluginFilesToRemotePluginFiles(IList<PluginFile> pluginFiles)
        {
            var output = new List<RemotePluginFile>(pluginFiles.Count());
            output.AddRange(
                pluginFiles.Select(pluginFile => new RemotePluginFile { Name = new FileInfo(pluginFile.Path).Name, Checksum = pluginFile.Checksum }));

            return output;
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

        private void DependenciesButtonClick(object sender, EventArgs e)
        {
            var dialog = new DependenciesForm(dependencies);

            if (dialog.ShowDialog(this) == DialogResult.Cancel)
            {
                dependencies = dialog.Dependencies;
            }
        }

        private void UpdateSaveButtonStatus()
        {
            var enabled = files.Any();

            if (nameTextBox.Text.Length < 1)
            {
                enabled = false;
            }

            if (linkTextBox.Text.Length < 1)
            {
                enabled = false;
            }

            if (siteAndAuthorComboBox.SelectedItem == null)
            {
                enabled = false;
            }

            if (descriptionTextBox.Text.Length < 1)
            {
                enabled = false;
            }

            saveButton.Enabled = enabled;
        }

        private void NameTextBoxTextChanged(object sender, EventArgs e)
        {
            UpdateSaveButtonStatus();
        }

        private void LinkTextBoxTextChanged(object sender, EventArgs e)
        {
            UpdateSaveButtonStatus();
        }

        private void SiteAndAuthorComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSaveButtonStatus();
        }

        private void SaveButtonClick(object sender, EventArgs e)
        {
            var link = linkTextBox.Text.Trim();
            var author = ((ComboBoxItem<Author>)siteAndAuthorComboBox.SelectedItem).Value;
            if (!ValidateLinkAndAuthor(link, author))
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

            var remotePlugin = new RemotePlugin { Name = name, Link = link, Author = author, Description = description };
            foreach (var file in files)
            {
                remotePlugin.PluginFiles.Add(file);
            }

            foreach (var dependency in dependencies)
            {
                remotePlugin.Dependencies.Add(dependency);
            }

            remotePluginRegistry.Add(remotePlugin);

            MessageBox.Show(
                this,
                LocalizationStrings.ThePluginHasBeenAddedToTheCentralDatabase,
                LocalizationStrings.PluginHasBeenAdded,
                MessageBoxButtons.OK,
                MessageBoxIcon.Asterisk,
                MessageBoxDefaultButton.Button1);

            ClearForm();
        }

        private void ClearForm()
        {
            nameTextBox.Text = string.Empty;
            linkTextBox.Text = string.Empty;
            descriptionTextBox.Text = string.Empty;
            siteAndAuthorComboBox.SelectedItem = null;
            files = new List<RemotePluginFile>();
            dependencies = new Collection<RemotePlugin>();
            saveButton.Enabled = false;
        }

        private bool ValidateLinkAndAuthor(string link, Author author)
        {
            var siteUri = new UriBuilder(author.Site);
            var linkUri = new UriBuilder(link);

            return linkUri.Host.EndsWith(siteUri.Host, StringComparison.OrdinalIgnoreCase);
        }

        private void DescriptionTextBoxTextChanged(object sender, EventArgs e)
        {
            UpdateSaveButtonStatus();
        }
    }
}
