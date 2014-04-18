namespace Sc4BuddyServer.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;

    using Sc4BuddyServer.Models;
    using Sc4BuddyServer.Services;

    public class PluginController : ApiController
    {
        private readonly IPluginRepository repository;

        public Plugin GetPlugin(Guid id)
        {
            return repository.GetPlugin(id);
        }

        public IEnumerable<Plugin> GetAllPlugins()
        {
            return repository.GetAllPlugins();
        }

        [Authorize]
        public HttpResponseMessage PostPlugin(Plugin plugin)
        {
            try
            {
                plugin = repository.AddPlugin(plugin);

                var response = Request.CreateResponse(HttpStatusCode.Created, plugin);

                // ReSharper disable once RedundantAnonymousTypePropertyName
                var uri = Url.Link("DefaultApi", new { Id = plugin.Id });
                response.Headers.Location = new Uri(uri);

                return response;
            }
            catch (Exception ex)
            {
                var response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                                   {
                                       Content =
                                           new StringContent(string.Format("Error message: {0}", ex.Message)),
                                   };

                return response;
            }
        }

        [Authorize]
        public void PutPlugin(Plugin plugin)
        {
            repository.UpdatePlugin(plugin);
        }

        [Authorize]
        public void DeletePlugin(Guid id)
        {
            repository.DeletePlugin(id);
        }
    }
}
