namespace Sc4BuddyServer.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Web.Http;

    using Sc4BuddyServer.Models;

    public class PluginController : ApiController
    {
        public Plugin GetPlugin(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Plugin> GetAllPlugins()
        {
            throw new NotImplementedException();
        }

        [Authorize]
        public HttpResponseMessage PostPlugin(Plugin plugin)
        {
            throw new NotImplementedException();
        }

        [Authorize]
        public void PutPlugin(Plugin plugin)
        {
            throw new NotImplementedException();
        }

        [Authorize]
        public void DeletePlugin(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
