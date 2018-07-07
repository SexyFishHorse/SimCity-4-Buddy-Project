namespace Nihei.SC4Buddy.Application.Control
{
    using System.Collections.Generic;
    using System.Drawing;

    public interface ISettingsController
    {
        bool ValidateGameLocationPath(string path);

        void CheckMainFolder();

        string SearchForGameLocation();

        IEnumerable<string> GetInstalledLanguages();

        IList<Bitmap> GetWallpapers();
    }
}