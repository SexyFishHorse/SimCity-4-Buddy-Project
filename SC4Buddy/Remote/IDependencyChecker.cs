namespace NIHEI.SC4Buddy.Remote
{
    using System.Collections.Generic;

    using NIHEI.SC4Buddy.Entities.Remote;
    using NIHEI.SC4Buddy.Model;

    public interface IDependencyChecker
    {
        List<RemotePlugin> CheckDependencies(UserFolder userFolder);
    }
}