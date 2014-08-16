namespace NIHEI.SC4Buddy.View.Elements
{
    using System.Globalization;
    using System.Windows.Forms;
    using NIHEI.SC4Buddy.Model;

    public class RemovalFileTypeCandidateListViewItem : ListViewItem
    {
        public RemovalFileTypeCandidateListViewItem(NonPluginFileTypeCandidateInfo candidateInfo)
        {
            CandidateInfo = candidateInfo;

            Text = candidateInfo.FileTypeInfo.DescriptiveName;

            SubItems.Add(new ListViewSubItem(this, candidateInfo.FileTypeInfo.Extension));
            SubItems.Add(new ListViewSubItem(this, candidateInfo.NumberOfEntities.ToString(CultureInfo.InvariantCulture)));
            SubItems.Add(new ListViewSubItem(this, candidateInfo.FileTypeInfo.Description));
        }

        public NonPluginFileTypeCandidateInfo CandidateInfo { get; set; }
    }
}