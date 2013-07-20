namespace NIHEI.SC4Buddy.View.Author
{
    using System;
    using System.Collections.Generic;
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

        private IList<RemotePluginFile> files;

        public AddPluginInformationForm()
        {
            authorRegistry = RemoteRegistryFactory.AuthorRegistry;

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
            var dialog = new AddFilesForm();

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                files = dialog.Files;
            }
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
                linkTextBox.Text = plugin.Link.Trim();
                descriptionTextBox.Text = plugin.Description.Trim();
                SelectAuthorInList(plugin.Author, plugin.Link);
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
            var dialog = new DependenciesForm();

            if (dialog.ShowDialog(this) == DialogResult.Cancel)
            {
                
            }
        }
    }
}
