namespace Sc4BuddyServer.Services
{
    using System;
    using System.Collections.Generic;

    using Sc4BuddyServer.Models;

    public interface IPluginRepository
    {
        Plugin GetPlugin(Guid id);

        IEnumerable<Plugin> GetAllPlugins();

        Plugin AddPlugin(Plugin plugin);

        void UpdatePlugin(Plugin plugin);

        void DeletePlugin(Guid id);
    }
}
