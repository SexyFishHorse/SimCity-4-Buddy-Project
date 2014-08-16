namespace NIHEI.SC4Buddy.View.Plugins
{
    using System.Collections.Generic;
    using System.Windows.Forms;
    using NIHEI.SC4Buddy.Model;

    public partial class RemoveUnnecessaryFilesForm : Form
    {
        public RemoveUnnecessaryFilesForm()
        {
            InitializeComponent();
        }

        public IEnumerable<NonPluginFileTypeCandidateInfo> CandidateInfos { get; set; }
    }
}
