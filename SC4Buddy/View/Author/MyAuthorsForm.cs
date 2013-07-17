namespace NIHEI.SC4Buddy.View.Author
{
    using System;
    using System.Linq;
    using System.Windows.Forms;

    using NIHEI.SC4Buddy.Control;
    using NIHEI.SC4Buddy.DataAccess.Remote;
    using NIHEI.SC4Buddy.Entities.Remote;
    using NIHEI.SC4Buddy.Localization;
    using NIHEI.SC4Buddy.View.Elements;

    public partial class MyAuthorsForm : Form
    {
        private readonly AuthorRegistry registry;

        public MyAuthorsForm()
        {
            registry = RemoteRegistryFactory.AuthorRegistry;
            InitializeComponent();
        }

        private void CloseButtonClick(object sender, EventArgs e)
        {
            Close();
        }

        private void ClearButtonClick(object sender, EventArgs e)
        {
            usernameTextBox.Text = string.Empty;
            siteComboBox.SelectedIndex = -1;
            siteComboBox.Text = string.Empty;
            addButton.Enabled = false;
            removeButton.Enabled = false;
            updateButton.Enabled = false;
            authorsListView.SelectedItems.Clear();
        }

        private void MyAuthorsFormLoad(object sender, EventArgs e)
        {
            UpdateAuthorListView();

            var sites = registry.Authors.Select(x => x.Site).Distinct().ToArray();
            var autoCompleteSource = new AutoCompleteStringCollection();
            autoCompleteSource.AddRange(sites);

            siteComboBox.AutoCompleteCustomSource = autoCompleteSource;
            // ReSharper disable CoVariantArrayConversion
            siteComboBox.Items.AddRange(sites);
            // ReSharper restore CoVariantArrayConversion
        }

        private void UsernameTextBoxTextChanged(object sender, EventArgs e)
        {
            UpdateClearButtonStatus();
            UpdateAddButtonStatus();
            UpdateUpdateButtonStatus();
            UpdateRemoveButtonStatus();
        }

        private void SiteComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateClearButtonStatus();
            UpdateAddButtonStatus();
            UpdateUpdateButtonStatus();
            UpdateRemoveButtonStatus();
        }

        private void SiteComboBoxTextUpdate(object sender, EventArgs e)
        {
            UpdateClearButtonStatus();
            UpdateAddButtonStatus();
            UpdateUpdateButtonStatus();
            UpdateRemoveButtonStatus();
        }

        private void UpdateRemoveButtonStatus()
        {
            removeButton.Enabled = authorsListView.SelectedItems.Count > 0;
        }

        private void UpdateUpdateButtonStatus()
        {
            updateButton.Enabled = authorsListView.SelectedItems.Count > 0
                                   && !string.IsNullOrWhiteSpace(usernameTextBox.Text)
                                   && !string.IsNullOrWhiteSpace(siteComboBox.Text);
        }

        private void UpdateAddButtonStatus()
        {
            addButton.Enabled = !string.IsNullOrWhiteSpace(usernameTextBox.Text)
                                 && !string.IsNullOrWhiteSpace(siteComboBox.Text)
                                && authorsListView.SelectedItems.Count == 0;
        }

        private void UpdateClearButtonStatus()
        {
            clearButton.Enabled =
                !string.IsNullOrEmpty(usernameTextBox.Text)
                || siteComboBox.SelectedIndex >= 0
                || !string.IsNullOrEmpty(siteComboBox.Text);
        }

        private void AuthorsListViewSelectedIndexChanged(object sender, EventArgs e)
        {
            if (authorsListView.SelectedItems.Count < 1)
            {
                return;
            }

            var author = ((ListViewItemWithObjectValue<Author>)authorsListView.SelectedItems[0]).Value;

            addButton.Enabled = false;

            usernameTextBox.Text = author.Name;
            siteComboBox.Text = author.Site;

            updateButton.Enabled = true;
            removeButton.Enabled = true;
        }

        private void RemoveButtonClick(object sender, EventArgs e)
        {
            if (authorsListView.SelectedItems.Count < 1)
            {
                return;
            }

            var author = ((ListViewItemWithObjectValue<Author>)authorsListView.SelectedItems[0]).Value;

            var numPlugins = author.Plugins.Count;
            string message;
            if (numPlugins > 0)
            {
                message =
                    string.Format(
                        LocalizationStrings.ThisUserHasNumPluginsInTheCentralDatabase,
                        numPlugins);
            }
            else
            {
                message = LocalizationStrings.ThisAuthorHasNoAssociatedPlugins;
            }

            if (MessageBox.Show(
                this,
                message,
                LocalizationStrings.ConfirmDeletionOfAuthor,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2) == DialogResult.No)
            {
                return;
            }

            if (numPlugins > 0)
            {
                author.UserId = 0;
                author.User = null;
                registry.Update(author);
            }
            else
            {
                registry.Delete(author);
            }

            UpdateAuthorListView();
        }

        private void UpdateAuthorListView()
        {
            var authors = registry.Authors.Where(x => x.UserId == SessionController.Instance.User.Id);

            authorsListView.BeginUpdate();
            authorsListView.Items.Clear();
            foreach (var author in authors)
            {
                var item = new ListViewItemWithObjectValue<Author>(author.Name, author);
                item.SubItems.Add(author.Site);
                authorsListView.Items.Add(item);
            }

            authorsListView.EndUpdate();

            authorsListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void UpdateButtonClick(object sender, EventArgs e)
        {
            if (authorsListView.SelectedItems.Count < 1)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(usernameTextBox.Text))
            {
                MessageBox.Show(
                    this,
                    LocalizationStrings.YouCannotSpecifyAnEmptyUsername,
                    LocalizationStrings.ValidationError,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1);
                return;
            }

            if (string.IsNullOrWhiteSpace(siteComboBox.Text))
            {
                MessageBox.Show(
                    this,
                    LocalizationStrings.YouCannotSpecifyAnEmptySite,
                    LocalizationStrings.ValidationError,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1);
                return;
            }

            try
            {
                var site = CleanupSiteUrl(siteComboBox.Text.Trim());

                var author =
                    ((ListViewItemWithObjectValue<Author>)authorsListView.SelectedItems[0]).Value;

                author.Name = usernameTextBox.Text.Trim();
                author.Site = site;

                registry.Update(author);

                UpdateAuthorListView();
            }
            catch (FormatException)
            {
                MessageBox.Show(
                    this,
                    LocalizationStrings.TheSiteUrlIsInvalid,
                    LocalizationStrings.ValidationError,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1);
            }
        }

        private string CleanupSiteUrl(string site)
        {
            if (!site.Contains("."))
            {
                throw new FormatException("Site URL does not contain a dot.");
            }

            var siteUri = new UriBuilder(site) { Path = string.Empty, Port = -1 };

            if (!siteUri.Scheme.Equals("http", StringComparison.OrdinalIgnoreCase)
                || !siteUri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase))
            {
                siteUri.Scheme = "http";
            }

            return siteUri.Uri.ToString();
        }

        private void AddButtonClick(object sender, EventArgs e)
        {
            var username = usernameTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show(
                    this,
                    LocalizationStrings.YouCannotSpecifyAnEmptyUsername,
                    LocalizationStrings.ValidationError,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1);
                return;
            }

            try
            {
                var site = CleanupSiteUrl(siteComboBox.Text.Trim());

                var author =
                    registry.Authors.FirstOrDefault(
                        x =>
                        x.Name.Equals(username, StringComparison.OrdinalIgnoreCase)
                        && x.Site.Replace("//www.", string.Empty)
                            .Equals(site.Replace("//www.", string.Empty), StringComparison.OrdinalIgnoreCase));

                if (author == null)
                {
                    author = new Author
                                 {
                                     Name = username,
                                     Site = site,
                                     User = SessionController.Instance.User,
                                     UserId = SessionController.Instance.User.Id
                                 };
                    registry.Add(author);
                    UpdateAuthorListView();
                    ClearButtonClick(sender, e);
                    return;
                }

                if (author.UserId > 0)
                {
                    MessageBox.Show(
                        this,
                        LocalizationStrings.ThisAuthorIsAlreadyClaimedByAnotherUser,
                        LocalizationStrings.ValidationError,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error,
                        MessageBoxDefaultButton.Button1);
                    return;
                }

                author.User = SessionController.Instance.User;
                author.UserId = SessionController.Instance.User.Id;
                registry.Update(author);
                UpdateAuthorListView();
                ClearButtonClick(sender, e);
            }
            catch (FormatException)
            {
                MessageBox.Show(
                    this,
                    LocalizationStrings.TheSiteUrlIsInvalid,
                    LocalizationStrings.ValidationError,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1);
            }
        }
    }
}
