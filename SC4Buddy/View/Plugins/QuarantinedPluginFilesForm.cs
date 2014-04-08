namespace NIHEI.SC4Buddy.View.Plugins
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;

    using Common.UI.Elements;
    using Control.Plugins;

    using NIHEI.SC4Buddy.Model;

    public partial class QuarantinedPluginFilesForm : Form
    {
        private readonly Plugin selectedPlugin;

        private readonly PluginFileController pluginFileController;

        public QuarantinedPluginFilesForm(
            Plugin selectedPlugin,
            PluginFileController pluginFileController)
        {
            this.selectedPlugin = selectedPlugin;
            this.pluginFileController = pluginFileController;

            InitializeComponent();
        }

        private void CancelButtonClick(object sender, EventArgs e)
        {
            pluginFileController.RevertChanges(selectedPlugin.PluginFiles.Cast<ModelBase>().ToList());

            Close();
        }

        private void QuarantinedPluginFilesFormLoad(object sender, EventArgs e)
        {
            var enabledFiles = this.selectedPlugin.PluginFiles
                .Where(x => x.QuarantinedFile == null)
                .ToList();
            var disabledFiles = this.selectedPlugin.PluginFiles
                .Where(x => x.QuarantinedFile != null)
                .ToList();

            PopulateListView(activeFilesListView, enabledFiles);
            PopulateListView(disabledFilesListView, disabledFiles);
        }

        private void PopulateListView(ListView listView, List<PluginFile> files)
        {
            if (!files.Any())
            {
                return;
            }

            listView.BeginUpdate();
            listView.Items.Clear();
            foreach (var file in files)
            {
                var filename = new FileInfo(file.Path).Name;
                listView.Items.Add(new ListViewItemWithObjectValue<PluginFile>(filename, file));
            }

            listView.EndUpdate();
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

            MoveItemsBetweenListViews(activeFilesListView, disabledFilesListView, items);
        }

        private void EnableButtonClick(object sender, EventArgs e)
        {
            var items = disabledFilesListView.SelectedItems;

            MoveItemsBetweenListViews(disabledFilesListView, activeFilesListView, items);
        }

        private void MoveItemsBetweenListViews(
            ListView originListView,
            ListView targetListView,
            IEnumerable items)
        {
            originListView.BeginUpdate();
            targetListView.BeginUpdate();

            foreach (ListViewItemWithObjectValue<PluginFile> item in items)
            {
                originListView.Items.Remove(item);
                targetListView.Items.Add(item);
            }

            targetListView.EndUpdate();
            originListView.EndUpdate();
        }

        private void OkButtonClick(object sender, EventArgs e)
        {
            var quarantined = disabledFilesListView.Items.Cast<ListViewItemWithObjectValue<PluginFile>>();
            var unquarantined = activeFilesListView.Items.Cast<ListViewItemWithObjectValue<PluginFile>>();
            pluginFileController.QuarantineFiles(quarantined.Select(x => x.Value));
            pluginFileController.UnquarantineFiles(unquarantined.Select(x => x.Value));
            pluginFileController.SaveChanges();

            Close();
        }
    }
}
