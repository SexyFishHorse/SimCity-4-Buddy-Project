namespace NIHEI.SC4Buddy.View.Author
{
    using System;
    using System.Windows.Forms;

    using NIHEI.SC4Buddy.Control;
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
        }
    }
}
