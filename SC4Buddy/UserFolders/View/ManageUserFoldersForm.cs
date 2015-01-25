namespace NIHEI.SC4Buddy.UserFolders.View
{
    using System;
    using System.Drawing;
    using System.Linq;
    using System.Resources;
    using System.Windows.Forms;
    using NIHEI.SC4Buddy.DataAccess;
    using NIHEI.SC4Buddy.Model;
    using NIHEI.SC4Buddy.Resources;
    using NIHEI.SC4Buddy.UserFolders.Control;
    using NIHEI.SC4Buddy.View.Elements;

    public partial class ManageUserFoldersForm : Form
    {
        public const int ErrorIconPadding = -18;

        private readonly ResourceManager localizationManager;

        private readonly IUserFolderController controller;

        public ManageUserFoldersForm()
        {
            InitializeComponent();

            controller = new UserFolderController(EntityFactory.Instance.Entities);
            localizationManager = new System.ComponentModel.ComponentResourceManager(typeof(ManageUserFoldersForm));
        }

        public UserFolderMode? Mode { get; set; }

        private UserFolder SelectedFolder { get; set; }

        private void UserFoldersFormLoad(object sender, EventArgs e)
        {
            ReloadUserFoldersListView();
        }

        private void ReloadUserFoldersListView()
        {
            var userFolders = controller.UserFolders.Where(x => !x.IsMainFolder);

            UserFoldersListView.BeginUpdate();
            UserFoldersListView.Items.Clear();
            foreach (var userFolder in userFolders)
            {
                UserFoldersListView.Items.Add(new UserFolderListViewItem(userFolder));
            }

            UserFoldersListView.EndUpdate();
        }

        private void BrowseButtonClick(object sender, EventArgs e)
        {
            var result = pathBrowseDialog.ShowDialog(this);

            if (result == DialogResult.OK)
            {
                pathTextBox.Text = pathBrowseDialog.SelectedPath;
            }
        }

        private void UserFoldersListViewSelectedIndexChanged(object sender, EventArgs e)
        {
            errorProvider.Clear();

            if (UserFoldersListView.SelectedItems.Count > 0)
            {
                SelectedFolder = ((UserFolderListViewItem)UserFoldersListView.SelectedItems[0]).UserFolder;

                pathTextBox.Text = SelectedFolder.FolderPath;
                pathBrowseDialog.SelectedPath = SelectedFolder.FolderPath;
                aliasTextBox.Text = SelectedFolder.Alias;
                startupFolderCheckbox.Checked = !SelectedFolder.IsMainFolder && SelectedFolder.IsStartupFolder;

                startupFolderCheckbox.Enabled = !SelectedFolder.IsMainFolder;
                EnableForm();
                removeButton.Enabled = true;
                Mode = UserFolderMode.Update;
            }
            else
            {
                EnableForm(false);
                SelectedFolder = null;

                pathTextBox.Text = string.Empty;
                aliasTextBox.Text = string.Empty;

                removeButton.Enabled = false;
            }
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

            ResetForm();
            ReloadUserFoldersListView();
        }

        private void ResetForm()
        {
            ClearForm();
            EnableForm(false);

            addButton.Enabled = true;
            removeButton.Enabled = false;
        }

        private void EnableForm(bool enabled = true)
        {
            pathTextBox.Enabled = enabled;
            browseButton.Enabled = enabled;
            aliasTextBox.Enabled = enabled;
            startupFolderCheckbox.Enabled = enabled;
            saveButton.Enabled = enabled;
            cancelButton.Enabled = enabled;
        }

        private void ClearForm()
        {
            pathBrowseDialog.SelectedPath = string.Empty;
            pathTextBox.Text = string.Empty;
            aliasTextBox.Text = string.Empty;

            SelectedFolder = null;
            Mode = null;
        }

        private void AddButtonClick(object sender, EventArgs e)
        {
            ClearForm();
            EnableForm();

            Mode = UserFolderMode.Add;
        }

        private void CloseButtonClick(object sender, EventArgs e)
        {
            Close();
        }

        private void SaveButtonClick(object sender, EventArgs e)
        {
            errorProvider.Clear();

            var hasErrors = false;

            switch (Mode)
            {
                case UserFolderMode.Add:
                    var newFolder = new UserFolder
                    {
                        FolderPath = pathTextBox.Text,
                        Alias = aliasTextBox.Text,
                        IsStartupFolder = startupFolderCheckbox.Checked
                    };

                    var pathOk = !controller.ValidatePath(newFolder.FolderPath, Guid.NewGuid());
                    var notMainFolder = !controller.IsNotGameFolder(newFolder.FolderPath);

                    if (pathOk || notMainFolder)
                    {
                        hasErrors = true;
                        errorProvider.SetIconPadding(pathTextBox, ErrorIconPadding);
                        errorProvider.SetError(pathTextBox, LocalizationStrings.PathError);
                    }

                    if (!controller.ValidateAlias(newFolder.Alias, Guid.Empty))
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
                    break;
                case UserFolderMode.Update:

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
                    SelectedFolder.IsStartupFolder = startupFolderCheckbox.Checked;

                    controller.Update(SelectedFolder);
                    break;
            }

            controller.SaveChanges();
            ResetForm();
            ReloadUserFoldersListView();
        }

        private void CancelButtonClick(object sender, EventArgs e)
        {
            ResetForm();
        }

        private void PathTextBoxLeave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(pathTextBox.Text))
            {
                pathTextBox.Text = localizationManager.GetString("pathTextBox.Text");
            }
        }

        private void PathTextBoxEnter(object sender, EventArgs e)
        {
            if (pathTextBox.Text.Trim() == localizationManager.GetString("pathTextBox.Text"))
            {
                pathTextBox.Text = string.Empty;
            }
        }

        private void PathTextBoxTextChanged(object sender, EventArgs e)
        {
            if (!pathTextBox.Focused)
            {
                if (string.IsNullOrEmpty(pathTextBox.Text))
                {
                    pathTextBox.Text = localizationManager.GetString("pathTextBox.Text");
                }
            }

            pathTextBox.ForeColor = pathTextBox.Text.Equals(localizationManager.GetString("pathTextBox.Text"))
                                        ? Color.Gray
                                        : Color.Black;
        }
    }
}
