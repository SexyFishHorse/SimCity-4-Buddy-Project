namespace NIHEI.SC4Buddy.View.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;
    
    using Common.UI.Elements;
    using Control.Plugins;
    
    using Entities;

    public partial class QuarantinedPluginFilesForm : Form
    {
        private readonly Plugin _selectedPlugin;

        private readonly PluginFileController pluginFileController;

        public QuarantinedPluginFilesForm(Plugin selectedPlugin, PluginFileController pluginFileController)
        {
            _selectedPlugin = selectedPlugin;
            this.pluginFileController = pluginFileController;

            InitializeComponent();
        }

        private void CancelButtonClick(object sender, EventArgs e)
        {
            Close();
        }

        private void QuarantinedPluginFilesForm_Load(object sender, EventArgs e)
        {
            var enabledFiles = _selectedPlugin.Files.Where(x => !x.Quarantined.HasValue || !x.Quarantined.Value).ToList();
            var disabledFiles = _selectedPlugin.Files.Where(x => x.Quarantined.HasValue && x.Quarantined.Value).ToList();

            PopulateListView(activeFilesListView, enabledFiles);
            PopulateListView(disabledFilesListView, disabledFiles);
        }

        private void PopulateListView(ListView listView, List<PluginFile> files)
        {
            if (files.Any())
            {
                listView.BeginUpdate();
                listView.Items.Clear();
                foreach (var file in files)
                {
                    var filename = new FileInfo(file.Path).Name;
                    listView.Items.Add(new ListViewItemWithObjectValue<PluginFile>(filename, file));
                }
            }
        }

        private void ActiveFilesListViewSelectedIndexChanged(object sender, EventArgs e)
        {
            disableButton.Enabled = activeFilesListView.SelectedItems.Count > 0;
        }

        private void DisabledFilesListViewSelectedIndexChanged(object sender, EventArgs e)
        {
            enableButton.Enabled = disabledFilesListView.SelectedItems.Count > 0;
        }

        private void DisableButtonClick(object sender, EventArgs e)
        {
            var items = activeFilesListView.SelectedItems;

            MoveItemsBetweenListViews(activeFilesListView, disabledFilesListView, items, true);
        }

        private void EnableButtonClick(object sender, EventArgs e)
        {
            var items = disabledFilesListView.SelectedItems;

            MoveItemsBetweenListViews(disabledFilesListView, activeFilesListView, items, false);
        }

        private void MoveItemsBetweenListViews(ListView originListView, ListView targetListView,
            ListView.SelectedListViewItemCollection items, bool quarantined)
        {
            originListView.BeginUpdate();
            targetListView.BeginUpdate();

            foreach (ListViewItemWithObjectValue<PluginFile> item in items)
            {
                originListView.Items.Remove(item);
                targetListView.Items.Add(item);

                pluginFileController.SetQuarantineStatus(item.Value, quarantined);
            }

            targetListView.EndUpdate();
            originListView.EndUpdate();
        }

        private void OkButtonClick(object sender, EventArgs e)
        {
            pluginFileController.SaveChanges();

            Close();
        }
    }
}
