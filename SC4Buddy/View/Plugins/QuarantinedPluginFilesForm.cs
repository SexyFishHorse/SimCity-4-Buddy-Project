using System.Collections.Generic;
using System.IO;
using System.Linq;
using NIHEI.Common.UI.Elements;
using NIHEI.SC4Buddy.Entities;

namespace NIHEI.SC4Buddy.View.Plugins
{
    using System;
    using System.Windows.Forms;

    public partial class QuarantinedPluginFilesForm : Form
    {
        private readonly Plugin _selectedPlugin;

        public QuarantinedPluginFilesForm(Plugin selectedPlugin)
        {
            _selectedPlugin = selectedPlugin;

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
    }
}
