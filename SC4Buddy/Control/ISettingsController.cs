namespace NIHEI.SC4Buddy.Control
{
    using System.Collections.Generic;
    using System.Drawing;

    public interface ISettingsController
    {
        bool ValidateGameLocationPath(string path);

        void UpdateMainFolder();

        string SearchForGameLocation();

        List<string> GetInstalledLanguages();

        List<Bitmap> GetWallpapers();
    }
}