namespace NIHEI.SC4Buddy.Remote
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NIHEI.SC4Buddy.Model;

    public interface IDependencyChecker
    {
        Task<IEnumerable<Plugin>> CheckDependenciesAsync(UserFolder userFolder);
    }
}