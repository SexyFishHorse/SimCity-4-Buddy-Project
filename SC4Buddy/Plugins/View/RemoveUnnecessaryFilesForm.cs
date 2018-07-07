namespace Nihei.SC4Buddy.Plugins.View
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows.Forms;
    using Nihei.SC4Buddy.Model;
    using Nihei.SC4Buddy.View.Elements;

    public partial class RemoveUnnecessaryFilesForm : Form
    {
        public RemoveUnnecessaryFilesForm()
        {
            InitializeComponent();
        }

        public Collection<NonPluginFileTypeCandidateInfo> ToBeRemoved { get; set; }

        public void SetCandidateInfos(IEnumerable<NonPluginFileTypeCandidateInfo> candidateInfos)
        {
            fileTypesListView.BeginUpdate();
            fileTypesListView.Items.Clear();

            foreach (var candidateInfo in candidateInfos)
            {
                fileTypesListView.Items.Add(
                    new RemovalFileTypeCandidateListViewItem(candidateInfo));
            }

            fileTypesListView.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.ColumnContent);
            fileTypesListView.AutoResizeColumn(1, ColumnHeaderAutoResizeStyle.HeaderSize);
            fileTypesListView.AutoResizeColumn(2, ColumnHeaderAutoResizeStyle.HeaderSize);
            fileTypesListView.AutoResizeColumn(3, ColumnHeaderAutoResizeStyle.ColumnContent);
            fileTypesListView.EndUpdate();
        }

        private void RemoveSelectedButtonClick(object sender, System.EventArgs e)
        {
            var checkedItems = fileTypesListView.CheckedItems;

            var toBeRemoved = new Collection<NonPluginFileTypeCandidateInfo>();

            foreach (RemovalFileTypeCandidateListViewItem checkedItem in checkedItems)
            {
                toBeRemoved.Add(checkedItem.CandidateInfo);
            }

            ToBeRemoved = toBeRemoved;
        }
    }
}
