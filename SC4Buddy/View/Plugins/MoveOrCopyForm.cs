using System.Windows.Forms;

namespace NIHEI.SC4Buddy.View.Plugins
{
    using System;
    using System.Linq;
    using System.Reflection;

    using log4net;

    using NIHEI.SC4Buddy.Control.Plugins;
    using NIHEI.SC4Buddy.Control.UserFolders;
    using NIHEI.SC4Buddy.Model;
    using NIHEI.SC4Buddy.View.Elements;

    public partial class MoveOrCopyForm : Form
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly UserFolder currentUserFolder;

        private readonly UserFolderController userFolderController;

        private readonly PluginController pluginController;

        private readonly PluginFileController pluginFileController;

        private UserFolder selectedUserFolder;

        public event EventHandler PluginCopied;

        public event EventHandler PluginMoved;

        public event EventHandler ErrorDuringCopyOrMove;

        public MoveOrCopyForm(
            UserFolder currentUserFolder,
            UserFolderController userFolderController,
            PluginController pluginController,
            PluginFileController pluginFileController)
        {
            this.currentUserFolder = currentUserFolder;
            this.userFolderController = userFolderController;
            this.pluginController = pluginController;
            this.pluginFileController = pluginFileController;
            InitializeComponent();
        }

        public Plugin Plugin { get; set; }

        protected virtual void OnPluginCopied()
        {
            var handler = PluginCopied;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        protected virtual void OnPluginMoved()
        {
            var handler = PluginMoved;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        protected virtual void OnErrorDuringCopyOrMove()
        {
            var handler = ErrorDuringCopyOrMove;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void CancelButtonClick(object sender, EventArgs e)
        {
            Close();
        }

        private void MoveOrCopyFormLoad(object sender, EventArgs e)
        {
            userFolderListView.BeginUpdate();
            userFolderListView.Items.Clear();

            var userFolders = userFolderController.UserFolders;

            foreach (var userFolder in userFolders.Where(userFolder => !userFolder.Equals(currentUserFolder)))
            {
                userFolderListView.Items.Add(new ListViewItemWithObjectValue<UserFolder>(userFolder.Alias, userFolder));
            }

            userFolderListView.EndUpdate();
        }

        private void UserFolderListViewSelectedIndexChanged(object sender, EventArgs e)
        {
            if (userFolderListView.SelectedItems.Count > 0)
            {
                selectedUserFolder = ((ListViewItemWithObjectValue<UserFolder>)userFolderListView.SelectedItems[0]).Value;

                moveButton.Enabled = true;
                copyButton.Enabled = true;
            }
            else
            {
                moveButton.Enabled = false;
                copyButton.Enabled = false;
            }
        }

        private void CopyButtonClick(object sender, EventArgs e)
        {
            var copier = new PluginCopier(pluginController, pluginFileController, userFolderController);
            try
            {
                copier.CopyPlugin(Plugin, selectedUserFolder);
                OnPluginCopied();
            }
            catch (Exception ex)
            {
                Log.Error(
                    string.Format(
                        "Error during copy plugin {0} (id: {1}) to folder {2}",
                        Plugin.Name,
                        Plugin.Id,
                        selectedUserFolder.PluginFolderPath),
                    ex);
                OnErrorDuringCopyOrMove();
            }

            Close();
        }

        private void MoveButtonClick(object sender, EventArgs e)
        {
            var copier = new PluginCopier(pluginController, pluginFileController, userFolderController);
            try
            {
                copier.CopyPlugin(Plugin, selectedUserFolder, true);
                OnPluginMoved();
            }
            catch (Exception ex)
            {
                Log.Error(
                    string.Format(
                        "Error during moving plugin {0} (id: {1}) to folder {2}",
                        Plugin.Name,
                        Plugin.Id,
                        selectedUserFolder.PluginFolderPath),
                    ex);
                OnErrorDuringCopyOrMove();
            }

            Close();
        }
    }
}
