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

    public partial class AddPluginInformationForm : Form
    {
        private readonly AuthorRegistry authorRegistry;

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
            var authors = authorRegistry.Authors.Where(x => x.UserId == SessionController.Instance.User.Id);

            ReloadSiteAndAuthorComboBoxItems(authors);
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
