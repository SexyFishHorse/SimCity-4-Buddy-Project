namespace NIHEI.SC4Buddy.Plugins.Services
{
    using System.Collections.Generic;
    using NIHEI.SC4Buddy.Model;

    public interface IDependencyChecker
    {
        IEnumerable<Plugin> CheckDependencies(UserFolder userFolder);
    }
}
