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
        }
    }
}
