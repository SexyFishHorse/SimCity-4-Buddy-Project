namespace NIHEI.SC4Buddy.View.Author
{
    using System;
    using System.Windows.Forms;

    public partial class AddPluginInformationForm : Form
    {
        public AddPluginInformationForm()
        {
            InitializeComponent();
        }

        private void CancelButtonClick(object sender, EventArgs e)
        {
            Close();
        }


        private void ReloadSiteAndAuthorComboBoxItems(IEnumerable<Author> authors)
        {
            siteAndAuthorComboBox.BeginUpdate();
            siteAndAuthorComboBox.Items.Clear();
            foreach (var author in authors)
            {
                siteAndAuthorComboBox.Items.Add(
                    new ListViewItemWithObjectValue<Entities.Remote.Author>(
                        string.Format("{1} ({0})", author.Name, author.Site), author));
            }

            siteAndAuthorComboBox.EndUpdate();
        }
    }
}
