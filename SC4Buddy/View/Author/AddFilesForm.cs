namespace NIHEI.SC4Buddy.View.Author
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;

    using NIHEI.Common.IO;
    using NIHEI.Common.UI.Elements;
    using NIHEI.SC4Buddy.Entities.Remote;
    using NIHEI.SC4Buddy.Installer.FileHandlers;
    using NIHEI.SC4Buddy.Localization;

    public partial class AddFilesForm : Form
    {
        private readonly IList<RemotePluginFile> files;

        public AddFilesForm()
        {
            InitializeComponent();

            files = new List<RemotePluginFile>();
        }

        public IList<RemotePluginFile> Files
        {
            get
            {
                return files;
            }
        }

        private void AddButtonClick(object sender, EventArgs e)
        {
            if (selectFileDialog.ShowDialog(this) == DialogResult.Cancel)
            {
                return;
            }

            var filenames = selectFileDialog.FileNames;

            foreach (var fileInfo in filenames.Select(filename => new FileInfo(filename)))
            {
                if (IsArchive(fileInfo))
                {
                    AddArchiveToList(fileInfo);
                }
                else
                {
                    AddFileToList(fileInfo);
                }
            }

            UpdateListViewAndOkButton();
        }

        private void AddArchiveToList(FileInfo fileInfo)
        {
            BaseHandler handler;
            switch (fileInfo.Extension.ToUpper())
            {
                case ".ZIP":
                    handler = new ZipHandler();
                    break;
                case ".RAR":
                    handler = new RarHandler();
                    break;
                default:
                    MessageBox.Show(
                    this,
                    string.Format(LocalizationStrings.TheFiletypeXIsNotRecognizedAsAValidArchiveFiletype, fileInfo.Extension),
                    LocalizationStrings.UnsupportedFiletype,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1);
                    return;
            }

            var randomFileName = Path.GetRandomFileName();
            randomFileName = randomFileName.Substring(0, randomFileName.Length - 4) + DateTime.UtcNow.Ticks;
            var tempPath = Path.Combine(Path.GetTempPath(), "SC4Buddy", randomFileName);

            handler.FileInfo = fileInfo;
            handler.TempFolder = tempPath;

            handler.ExtractFilesToTemp();

            var tempFiles = Directory.EnumerateFiles(tempPath, "*", SearchOption.AllDirectories);

            var numExecutables = 0;
            foreach (var newFileInfo in tempFiles.Select(file => new FileInfo(file)))
            {
                if (newFileInfo.Extension.ToUpper().Equals(".EXE"))
                {
                    numExecutables++;
                    continue;
                }

                AddFileToList(newFileInfo);
            }

            if (numExecutables > 0)
            {
                MessageBox.Show(
                    this,
                    string.Format(LocalizationStrings.ThisArchiveContainsNumExecutableFiles, numExecutables),
                    LocalizationStrings.UnsupportedFiletype,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button1);
            }

            FileUtility.DeleteFolder(new DirectoryInfo(tempPath));
        }

        private bool IsArchive(FileInfo fileInfo)
        {
            var supportedArchiveTypes = new[] { ".rar", ".zip" };
            return supportedArchiveTypes.Any(type => fileInfo.Extension.Equals(type, StringComparison.OrdinalIgnoreCase));
        }

        private void UpdateListViewAndOkButton()
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

            okButton.Enabled = files.Any();
        }

        private void AddFileToList(FileInfo fileInfo)
        {
            if (fileInfo.Extension.ToUpper().Equals(".EXE"))
            {
                MessageBox.Show(
                    this,
                    LocalizationStrings.YouCannotAddExecutableFilesToTheListOfPluginFiles,
                    LocalizationStrings.InvalidFileTypeDetected,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1);
                return;
            }

            var file = new RemotePluginFile
                           {
                               Name = fileInfo.Name,
                               Checksum = Md5ChecksumUtility.CalculateChecksum(fileInfo.FullName).ToHex()
                           };
            files.Add(file);
        }

        private void CancelButtonClick(object sender, EventArgs e)
        {
            Close();
        }

        private void FilesListViewSelectedIndexChanged(object sender, EventArgs e)
        {
            removeButton.Enabled = filesListView.SelectedItems.Count > 0;
        }

        private void RemoveButtonClick(object sender, EventArgs e)
        {
            var selectedItems = filesListView.SelectedItems;

            foreach (var selectedItem in selectedItems)
            {
                files.Remove(((ListViewItemWithObjectValue<RemotePluginFile>)selectedItem).Value);
            }

            UpdateListViewAndOkButton();

            removeButton.Enabled = false;
        }
    }
}
