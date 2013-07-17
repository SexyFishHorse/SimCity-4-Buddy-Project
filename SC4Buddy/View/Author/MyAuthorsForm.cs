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
    }
}
