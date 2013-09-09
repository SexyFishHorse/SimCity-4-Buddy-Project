namespace NIHEI.SC4Buddy.Control
{
    using System.Collections.Generic;
    using System.Drawing;

    public interface ISettingsController
    {
        bool ValidateGameLocationPath(string path);

        void UpdateMainFolder();

        string SearchForGameLocation();

        IEnumerable<string> GetInstalledLanguages();

        IList<Bitmap> GetWallpapers();

        bool ValidatePathExists(string path);
    }
}