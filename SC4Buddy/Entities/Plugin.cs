namespace NIHEI.SC4Buddy.Entities
{
    using System.Linq;

    using NIHEI.SC4Buddy.DataAccess;
    using NIHEI.SC4Buddy.Entities.Remote;

    public partial class Plugin
    {
        public RemotePlugin RemotePlugin
        {
            get
            {
                return EntityFactory.Instance.RemoteEntities.Plugins.FirstOrDefault(x => RemotePluginId == x.Id);
            }
        }
    }
}
