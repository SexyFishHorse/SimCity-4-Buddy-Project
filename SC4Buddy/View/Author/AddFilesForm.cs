namespace NIHEI.SC4Buddy.View.Author
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows.Forms;

    using NIHEI.Common.IO;
    using NIHEI.Common.UI.Elements;
    using NIHEI.SC4Buddy.Entities.Remote;

    public partial class AddFilesForm : Form
    {
        private readonly IList<RemotePluginFile> files;

        public AddFilesForm()
        {
            InitializeComponent();

            files = new List<RemotePluginFile>();
        }

        private void UpdateListView()
        {
            filesListView.BeginUpdate();
            filesListView.Items.Clear();
            foreach (var file in files)
            {
                var item = new ListViewItemWithObjectValue<RemotePluginFile>(file.Name, file);
                item.SubItems.Add(file.Checksum);
                filesListView.Items.Add(item);
            }

            filesListView.EndUpdate();
            filesListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void AddFileToList(string filename)
        {
            var fileinfo = new FileInfo(filename);
            var file = new RemotePluginFile
                           {
                               Name = fileinfo.Name,
                               Checksum = Md5ChecksumUtility.CalculateChecksum(fileinfo.FullName).ToHex()
                           };
            files.Add(file);

            Console.WriteLine("\tadded");
        }

        private void CancelButtonClick(object sender, EventArgs e)
        {
            Close();
        }
    }
}
