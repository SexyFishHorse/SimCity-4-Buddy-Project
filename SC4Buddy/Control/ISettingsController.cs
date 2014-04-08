namespace NIHEI.SC4Buddy.Control
{
    using System.Collections.Generic;
    using System.Drawing;

    public interface ISettingsController
    {
        string DefaultQuarantinedFilesPath { get; }

        bool ValidateGameLocationPath(string path);

        void CheckMainFolder();

        string SearchForGameLocation();

        IEnumerable<string> GetInstalledLanguages();

        IList<Bitmap> GetWallpapers();
    }
}