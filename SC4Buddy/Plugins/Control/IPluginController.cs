namespace NIHEI.SC4Buddy.Plugins.Control
{
    using System.Collections.Generic;
    using NIHEI.SC4Buddy.Model;

    public interface IPluginController
    {
        ICollection<Plugin> Plugins { get; }

        void Add(Plugin plugin, bool save = true);

        void SaveChanges();

        void Delete(Plugin plugin, bool save = true);

        int RemoveEmptyPlugins();

        void RevertChanges(Plugin selectedPlugin);
    }
}