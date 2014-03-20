namespace NIHEI.SC4Buddy.View.UserFolders
{
    using System;
    using System.Drawing;
    using System.Linq;
    using System.Resources;
    using System.Windows.Forms;

    using NIHEI.SC4Buddy.Control.UserFolders;
    using NIHEI.SC4Buddy.DataAccess;
    using NIHEI.SC4Buddy.Localization;
    using NIHEI.SC4Buddy.Model;
    using NIHEI.SC4Buddy.View.Elements;

    public partial class ManageUserFoldersForm : Form
    {
        public const int ErrorIconPadding = -18;

        private readonly ResourceManager localizationManager;

        private readonly UserFolderController controller;

        public ManageUserFoldersForm()
        {
            InitializeComponent();

            controller = new UserFolderController(EntityFactory.Instance.Entities);
            localizationManager = new System.ComponentModel.ComponentResourceManager(typeof(ManageUserFoldersForm));
        }

        private UserFolder SelectedFolder { get; set; }

        private void UserFoldersFormLoad(object sender, EventArgs e)
        {
            ReloadUserFoldersListView();
        }

        private void ReloadUserFoldersListView()
        {
            var userFolders = controller.UserFolders.Where(x => x.Id != 1);

            UserFoldersListView.BeginUpdate();
            UserFoldersListView.Items.Clear();
            foreach (var userFolder in userFolders)
            {
                UserFoldersListView.Items.Add(new UserFolderListViewItem(userFolder));
            }

            UserFoldersListView.EndUpdate();
        }

        private void PathClick(object sender, EventArgs e)
        {
            var result = pathBrowseDialog.ShowDialog(this);

            if (result == DialogResult.OK)
            {
                pathTextBox.Text = pathBrowseDialog.SelectedPath;
            }

            CheckFormFields();
        }

        private void UserFoldersListViewSelectedIndexChanged(object sender, EventArgs e)
        {
            errorProvider.Clear();

            if (UserFoldersListView.SelectedItems.Count > 0)
            {
                SelectedFolder = ((UserFolderListViewItem)UserFoldersListView.SelectedItems[0]).UserFolder;

                pathTextBox.Text = SelectedFolder.FolderPath;
                aliasTextBox.Text = SelectedFolder.Alias;
                updateButton.Enabled = true;
                removeButton.Enabled = true;
                clearButton.Enabled = true;
            }
            else
            {
                SelectedFolder = null;

                pathTextBox.Text = string.Empty;
                aliasTextBox.Text = string.Empty;
                updateButton.Enabled = false;
                removeButton.Enabled = false;
            }
        }

        private void PathTextBoxTextChanged(object sender, EventArgs e)
        {
            pathTextBox.ForeColor = pathTextBox.Text.Equals(localizationManager.GetString("pathTextBox.Text"))
                                        ? Color.Gray
                                        : Color.Black;
            CheckFormFields();
        }

        private void CheckFormFields()
        {
            errorProvider.Clear();

            addButton.Enabled =
                pathTextBox.Text.Length > 0
                && !pathTextBox.Text.Equals(localizationManager.GetString("pathTextBox.Text"))
                && aliasTextBox.Text.Length > 0
                && SelectedFolder == null;

            clearButton.Enabled =
                (pathTextBox.Text.Length > 0
                && !pathTextBox.Text.Equals(localizationManager.GetString("pathTextBox.Text")))
                || aliasTextBox.Text.Length > 0;
        }

        private void RemoveButtonClick(object sender, EventArgs e)
        {
            var text = LocalizationStrings.ConfirmUserFolderDeletionMessageBeforeFolderName + " \""
                       + SelectedFolder.Alias + "\"? (" + LocalizationStrings.UserFolderContentIsNotDeleted + ")";
            var result = MessageBox.Show(
                this,
                text,
                LocalizationStrings.ConfirmUserFolderDeletion,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button2);

            if (result != DialogResult.Yes)
            {
                return;
            }

            controller.Delete(SelectedFolder);

            ClearForm();
            ReloadUserFoldersListView();
        }

        private void ClearForm()
        {
            pathBrowseDialog.SelectedPath = string.Empty;
            pathTextBox.Text = localizationManager.GetString("pathTextBox.Text");
            aliasTextBox.Text = string.Empty;

            SelectedFolder = null;

            addButton.Enabled = false;
            removeButton.Enabled = false;
            updateButton.Enabled = false;
            clearButton.Enabled = false;
        }

        private void AddButtonClick(object sender, EventArgs e)
        {
            var newFolder = new UserFolder { FolderPath = pathTextBox.Text, Alias = aliasTextBox.Text };
            var hasErrors = false;

            var pathOk = !controller.ValidatePath(newFolder.FolderPath);
            var notMainFolder = !controller.IsNotGameFolder(newFolder.FolderPath);

            if (pathOk || notMainFolder)
            {
                hasErrors = true;
                errorProvider.SetIconPadding(pathTextBox, ErrorIconPadding);
                errorProvider.SetError(pathTextBox, LocalizationStrings.PathError);
            }

            if (!controller.ValidateAlias(newFolder.Alias))
            {
                hasErrors = true;
                errorProvider.SetIconPadding(aliasTextBox, ErrorIconPadding);
                errorProvider.SetError(aliasTextBox, LocalizationStrings.AliasError);
            }

            if (hasErrors)
            {
                return;
            }

            controller.Add(newFolder);
            ClearForm();
            ReloadUserFoldersListView();
        }

        private void AliasTextBoxTextChanged(object sender, EventArgs e)
        {
            CheckFormFields();
        }

        private void UpdateButtonClick(object sender, EventArgs e)
        {
            errorProvider.Clear();

            var hasErrors = false;

            if (!controller.ValidatePath(pathTextBox.Text, SelectedFolder.Id))
            {
                hasErrors = true;
                errorProvider.SetIconPadding(pathTextBox, ErrorIconPadding);
                errorProvider.SetError(pathTextBox, LocalizationStrings.PathError);
            }

            if (!controller.ValidateAlias(aliasTextBox.Text, SelectedFolder.Id))
            {
                hasErrors = true;
                errorProvider.SetIconPadding(aliasTextBox, ErrorIconPadding);
                errorProvider.SetError(aliasTextBox, LocalizationStrings.AliasError);
            }

            if (hasErrors)
            {
                return;
            }

            SelectedFolder.FolderPath = pathTextBox.Text;
            SelectedFolder.Alias = aliasTextBox.Text;

            controller.Update(SelectedFolder);
            ClearForm();
            ReloadUserFoldersListView();
        }

        private void ClearButtonClick(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void CloseButtonClick(object sender, EventArgs e)
        {
            Close();
        }

        private void SaveButtonClick(object sender, EventArgs e)
        {
            controller.SaveChanges();
        }
    }
}
