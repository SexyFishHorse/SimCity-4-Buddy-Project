namespace NIHEI.SC4Buddy.Plugins.Installer
{
    using System;
    using System.Collections.Generic;
    using NIHEI.SC4Buddy.Model;

    public class PluginFileComparer : IEqualityComparer<PluginFile>
    {
        public bool Equals(PluginFile x, PluginFile y)
        {
            return x.Path.Equals(y.Path, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(PluginFile obj)
        {
            return obj.Path.GetHashCode();
        }
    }
}