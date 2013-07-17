﻿namespace NIHEI.SC4Buddy.View.Author
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
        }
    }
}