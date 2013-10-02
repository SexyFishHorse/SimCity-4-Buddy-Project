namespace NIHEI.SC4Buddy.View.Admin.ManagePlugins
{
    using System.Collections.Generic;
    using System.Data.Objects.DataClasses;
    using System.IO;
    using System.Windows.Forms;

    using NIHEI.Common.UI.Elements;
    using NIHEI.SC4Buddy.Entities.Remote;

    public partial class ManagePluginFilesForm : Form
    {
        public RemotePlugin Plugin { get; set; }

        private EntityCollection<RemotePluginFile> pluginFiles;

        public ManagePluginFilesForm(RemotePlugin remotePlugin)
        {
            InitializeComponent();
            Plugin = remotePlugin;
            PluginFiles = Plugin.PluginFiles;
        }

        public EntityCollection<RemotePluginFile> PluginFiles
        {
            get
            {
                return pluginFiles;
            }

            private set
            {
                pluginFiles = value;

                UpdateListView(pluginFiles);
            }
        }

        private void UpdateListView(IEnumerable<RemotePluginFile> pluginFiles)
        {
            filesListView.BeginUpdate();
            filesListView.Items.Clear();

            foreach (var file in pluginFiles)
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

            pluginFiles.Remove(selectedItem);
        }
    }
}
