namespace NIHEI.SC4Buddy.View.Author
{
    using System;
    using System.Windows.Forms;

    public partial class MyAuthorsForm : Form
    {
        public MyAuthorsForm()
        {
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
