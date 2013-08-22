namespace NIHEI.SC4Buddy.Control
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Microsoft.VisualBasic.FileIO;

    using NIHEI.SC4Buddy.Control.UserFolders;
    using NIHEI.SC4Buddy.Entities;
    using NIHEI.SC4Buddy.Installer.FileHandlers;

    using SearchOption = System.IO.SearchOption;

    public class NonPluginFilesScanner
    {
        private static IList<string> GetFoldersToDelete(UserFolder userFolder)
        {
            var folders = Directory.EnumerateDirectories(userFolder.PluginFolderPath, "*", SearchOption.AllDirectories).ToList();
            var foldersToDelete =
                folders.Where(x => !new DirectoryInfo(x).EnumerateFiles("*", SearchOption.AllDirectories).Any()).ToList();
            return foldersToDelete;
        }

        private static IEnumerable<string> GetFilesToDelete(UserFolder userFolder)
        {
            var files = Directory.EnumerateFiles(userFolder.PluginFolderPath, "*", SearchOption.AllDirectories);
            var filesToDelete =
                files.Where(
                    x =>
                    !BaseHandler.IsPluginFile(x) && !UserFolderController.IsBackgroundImage(x, userFolder)
                    && !UserFolderController.IsDamnFile(userFolder, x)).ToList();
            return filesToDelete;
        }
    }
}
