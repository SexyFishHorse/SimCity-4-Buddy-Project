namespace NIHEI.SC4Buddy.View.Author
{
    using System;
    using System.Linq;
    using System.Windows.Forms;

    using NIHEI.SC4Buddy.Control;
    using NIHEI.SC4Buddy.DataAccess.Remote;
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
            var authors = registry.Authors.Where(x => x.UserId == SessionController.Instance.User.Id);

            authorsListView.BeginUpdate();
            authorsListView.Items.Clear();
            foreach (var author in authors)
            {
                var item = new ListViewItemWithObjectValue<Entities.Remote.Author>(author.Name, author);
                item.SubItems.Add(author.Site);
                authorsListView.Items.Add(item);
            }

            authorsListView.EndUpdate();

            authorsListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

            var sites = registry.Authors.Select(x => x.Site).Distinct().ToArray();
            var autoCompleteSource = new AutoCompleteStringCollection();
            autoCompleteSource.AddRange(sites);

            siteComboBox.AutoCompleteCustomSource = autoCompleteSource;
            siteComboBox.Items.AddRange(sites);
        }

        private void UsernameTextBoxTextChanged(object sender, EventArgs e)
        {
            UpdateClearButtonStatus();
        }

        private void SiteComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateClearButtonStatus();
        }

        private void SiteComboBoxTextUpdate(object sender, EventArgs e)
        {
            UpdateClearButtonStatus();
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

            var author = ((ListViewItemWithObjectValue<Entities.Remote.Author>)authorsListView.SelectedItems[0]).Value;

            addButton.Enabled = false;

            usernameTextBox.Text = author.Name;
            siteComboBox.Text = author.Site;

            updateButton.Enabled = true;
            removeButton.Enabled = true;
        }
    }
}
