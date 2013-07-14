namespace NIHEI.SC4Buddy.View.Plugins
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Windows.Forms;

    using NIHEI.SC4Buddy.View.Elements;

    public partial class ReadmeFilesForm : Form
    {
        public ReadmeFilesForm(IEnumerable<FileInfo> readmeFiles)
        {
            InitializeComponent();

            foreach (var readmeFile in readmeFiles)
            {
                readmeFilesListView.Items.Add(
                    new ListViewItemWithObjectValue<FileInfo>(readmeFile.Name, readmeFile));
            }
        }

        private void ReadmeFilesListViewSelectedIndexChanged(object sender, System.EventArgs e)
        {
            openButton.Enabled = readmeFilesListView.SelectedItems.Count > 0;
        }

        private void OpenButtonClick(object sender, System.EventArgs e)
        {
            var selectedValue = (ListViewItemWithObjectValue<FileInfo>)readmeFilesListView.SelectedItems[0];
            Process.Start(selectedValue.Value.FullName);
        }

        private void CloseButtonClick(object sender, System.EventArgs e)
        {
            Close();
        }
    }
}
