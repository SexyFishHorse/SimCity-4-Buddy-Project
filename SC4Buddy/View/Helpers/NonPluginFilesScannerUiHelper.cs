namespace NIHEI.SC4Buddy.View.Helpers
{
    using System.Windows.Forms;

    using NIHEI.SC4Buddy.Localization;
    using NIHEI.SC4Buddy.View.UserFolders;

    public static class NonPluginFilesScannerUiHelper
    {
        public static void ShowThereAreNoEntitiesToRemoveDialog(Form parentForm)
        {
            MessageBox.Show(
                    parentForm,
                    LocalizationStrings.ThereAreNoNonPluginFilesOrEmptyFoldersToRemove,
                    LocalizationStrings.NoNonPluginFilesDetected,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
        }

        public static bool ShowConfirmDialog(UserFolderForm parentForm, int numFiles, int numFolders)
        {
            return MessageBox.Show(
                parentForm,
                string.Format(LocalizationStrings.ThisWillRemoveNumFilesAndAtLeastNumFolders, numFiles, numFolders),
                LocalizationStrings.ConfirmDeletionOfNonPluginFiles,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2) == DialogResult.Yes;
        }

        public static void ShowRemovalSummary(UserFolderForm parentForm, int numFiles, int numFolders)
        {
            var message = string.Format(LocalizationStrings.NumFilesAndNumFoldersWereRemoved, numFiles, numFolders);

            MessageBox.Show(
                parentForm,
                message,
                LocalizationStrings.NonPluginFilesDeleted,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1);
        }
    }
}
