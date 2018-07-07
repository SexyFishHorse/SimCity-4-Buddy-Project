namespace Nihei.SC4Buddy.Plugins.Services
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using Nihei.SC4Buddy.Model;

    public interface IDependencyChecker
    {
        IEnumerable<Asser.Sc4Buddy.Server.Api.V1.Models.Plugin> CheckDependencies(
            UserFolder userFolder,
            ICollection<Plugin> mainUserFolderPlugins,
            BackgroundWorker backgroundWorker);
    }
}
