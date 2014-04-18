namespace Sc4BuddyServer.Controllers
{
    using System;
    using System.Collections.Generic;

    using Sc4BuddyServer.Models;
    using Sc4BuddyServer.Services;

    public class PluginRepository : IPluginRepository
    {
        public Plugin GetPlugin(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Plugin> GetAllPlugins()
        {
            throw new NotImplementedException();
        }

        public Plugin AddPlugin(Plugin plugin)
        {
            throw new NotImplementedException();
        }

        public void UpdatePlugin(Plugin plugin)
        {
            throw new NotImplementedException();
        }

        public void DeletePlugin(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
