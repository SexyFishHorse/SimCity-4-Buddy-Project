namespace Nihei.SC4Buddy.View.Helpers
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Forms;
    using Nihei.SC4Buddy.Model;
    using Nihei.SC4Buddy.Plugins.Control;
    using Nihei.SC4Buddy.Plugins.View;
    using Nihei.SC4Buddy.Resources;

    public class NonPluginFilesScannerUi
    {
        private readonly NonPluginFilesScanner scanner;

        public NonPluginFilesScannerUi(string storageLocation)
        {
            scanner = new NonPluginFilesScanner(storageLocation);
        }

        public UserFolder UserFolder { get; set; }

        public ICollection<NonPluginFileTypeCandidateInfo> RemovalCandidateInfos { get; set; }

        public Collection<NonPluginFileTypeCandidateInfo> ToBeRemoved { get; set; }

        public bool ShowDoYouWantToScanForNonPluginFiles(Form parentForm)
        {
            return MessageBox.Show(
                parentForm,
                LocalizationStrings.DoYouWantToScanForAndRemoveNonPluginFiles,
                LocalizationStrings.RemoveNonPluginFiles,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Asterisk,
                MessageBoxDefaultButton.Button1) == DialogResult.Yes;
        }

        public void ScanForCandidates()
        {
            RemovalCandidateInfos = scanner.GetFilesAndFoldersToRemove(UserFolder);
        }

        public void ShowThereAreNoEntitiesToRemoveDialog(Form parentForm)
        {
            MessageBox.Show(
                parentForm,
                LocalizationStrings.ThereAreNoNonPluginFilesOrEmptyFoldersToRemove,
                LocalizationStrings.NoNonPluginFilesDetected,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        public bool ShowConfirmDialog(Form parentForm)
        {
            var dialog = new RemoveUnnecessaryFilesForm();
            dialog.SetCandidateInfos(RemovalCandidateInfos);

            var continueDeletion = dialog.ShowDialog(parentForm) == DialogResult.OK;

            if (continueDeletion)
            {
                ToBeRemoved = dialog.ToBeRemoved;
            }

            return continueDeletion;
        }

        public void ShowRemovalSummary(Form parentForm, NonPluginFileRemovalSummary removalSummary)
        {
            var message = string.Format(LocalizationStrings.NumFilesAndNumFoldersWereRemoved, removalSummary.NumFilesRemoved, removalSummary.NumFoldersRemoved);

            if (removalSummary.Errors.Any())
            {
                message = string.Format(LocalizationStrings.NErrorsOccuredCheckTheLogForFurtherDetails, message, removalSummary.Errors.Count);
            }

            MessageBox.Show(
                parentForm,
                message,
                LocalizationStrings.NonPluginFilesDeleted,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1);
        }

        public void RemoveNonPluginFilesAndShowSummary(Form form)
        {
            var removalSummary = scanner.RemoveNonPluginFiles(UserFolder, ToBeRemoved.Select(x => x.FileTypeInfo));
            ShowRemovalSummary(form, removalSummary);
        }
    }
}
