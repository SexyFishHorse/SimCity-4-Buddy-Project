namespace NIHEI.SC4Buddy.View.Helpers
{
    using System.Windows.Forms;

    using NIHEI.SC4Buddy.Localization;

    public static class NonPluginFilesScannerUiHelper
    {
        public static bool ShowDoYouWantToScanForNonPluginFiles(Form parentForm)
        {
            return MessageBox.Show(
                parentForm,
                LocalizationStrings.DoYouWantToScanForAndRemoveNonPluginFiles,
                LocalizationStrings.RemoveNonPluginFiles,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Asterisk,
                MessageBoxDefaultButton.Button1) == DialogResult.Yes;
        }

        public static void ShowThereAreNoEntitiesToRemoveDialog(Form parentForm)
        {
            MessageBox.Show(
                parentForm,
                LocalizationStrings.ThereAreNoNonPluginFilesOrEmptyFoldersToRemove,
                LocalizationStrings.NoNonPluginFilesDetected,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        public static bool ShowConfirmDialog(Form parentForm, int numFiles, int numFolders)
        {
            return MessageBox.Show(
                parentForm,
                string.Format(LocalizationStrings.ThisWillRemoveNumFilesAndAtLeastNumFolders, numFiles, numFolders),
                LocalizationStrings.ConfirmDeletionOfNonPluginFiles,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2) == DialogResult.Yes;
        }

        public static void ShowRemovalSummary(Form parentForm, int numFiles, int numFolders)
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
