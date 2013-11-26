namespace NIHEI.SC4Buddy.View.Admin.ManagePlugins
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Windows.Forms;

    using NIHEI.Common.IO;
    using NIHEI.Common.UI.Elements;
    using NIHEI.SC4Buddy.Entities.Remote;

    public partial class ManagePluginFilesForm : Form
    {
        public ManagePluginFilesForm(IEnumerable<RemotePluginFile> files)
        {
            InitializeComponent();

            PluginFiles = new Collection<RemotePluginFile>();

            foreach (var file in files)
            {
                PluginFiles.Add(file);
            }

            UpdateListView(PluginFiles);
        }

        public ICollection<RemotePluginFile> PluginFiles { get; set; }

        private void UpdateListView(IEnumerable<RemotePluginFile> files)
        {
            filesListView.BeginUpdate();
            filesListView.Items.Clear();

            foreach (var file in files)
            {
                var item = new ListViewItemWithObjectValue<RemotePluginFile>(Path.GetFileName(file.Name), file);
                item.SubItems.Add(file.Checksum);
                filesListView.Items.Add(item);
            }

            filesListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            filesListView.EndUpdate();
        }

        private void FilesListViewSelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (filesListView.SelectedItems.Count < 1)
            {
                removeButton.Enabled = false;
            }

            removeButton.Enabled = true;
        }

        private void RemoveButtonClick(object sender, System.EventArgs e)
        {
            var selectedItem = ((ListViewItemWithObjectValue<RemotePluginFile>)filesListView.SelectedItems[0]).Value;

            PluginFiles.Remove(selectedItem);

            UpdateListView(PluginFiles);

            removeButton.Enabled = false;
        }

        private void AddButtonClick(object sender, System.EventArgs e)
        {
            if (selectFileDialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            var files = selectFileDialog.FileNames;

            foreach (var file in files)
            {
                var remoteFile = new RemotePluginFile
                                     {
                                         Name = Path.GetFileName(file),
                                         Checksum = Md5ChecksumUtility.CalculateChecksum(file).ToHex(),
                                     };

                if (!PluginFiles.Contains(remoteFile))
                {
                    PluginFiles.Add(remoteFile);
                }
            }

            UpdateListView(PluginFiles);
        }
    }
}
