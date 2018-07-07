namespace Asser.Sc4Buddy.Server.Api.V1.Client
{
    using System;
    using System.Collections.Generic;
    using Asser.Sc4Buddy.Server.Api.V1.Models;
    using Nihei.Common.Net;
    using RestSharp;

    public class BuddyServerClient : IBuddyServerClient
    {
        public const int MaxFilesPerPage = 100;

        private readonly IRestClient client;

        public BuddyServerClient(IRestClient client)
        {
            this.client = client;
        }

        public IEnumerable<File> GetAllFiles()
        {
            var page = 1;
            do
            {
                var request = new RestRequest("files", Method.GET) { RequestFormat = DataFormat.Json };
                request.AddQueryParameter("page", page + string.Empty);
                request.AddQueryParameter("perPage", MaxFilesPerPage + string.Empty);

                var response = client.Get<List<File>>(request);

                if (response.ErrorException != null)
                {
                    throw response.ErrorException;
                }

                if (response.StatusCode.IsSuccess())
                {
                    foreach (var file in response.Data)
                    {
                        yield return file;
                    }

                    if (response.Data.Count < MaxFilesPerPage)
                    {
                        yield break;
                    }

                    page++;
                }
                else
                {
                    yield break;
                }
            }
            while (true);
        }

        public Plugin GetPlugin(Guid pluginId)
        {
            var request = new RestRequest("plugins/{pluginId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddUrlSegment("pluginId", pluginId.ToString());

            var response = client.Get<Plugin>(request);

            if (response.ErrorException != null)
            {
                throw response.ErrorException;
            }

            return response.Data;
        }

        public IEnumerable<Plugin> GetAllPlugins()
        {
            var page = 1;
            do
            {
                var request = new RestRequest("plugins", Method.GET) { RequestFormat = DataFormat.Json };
                request.AddQueryParameter("page", page + string.Empty);
                request.AddQueryParameter("perPage", MaxFilesPerPage + string.Empty);

                var response = client.Get<List<Plugin>>(request);

                if (response.ErrorException != null)
                {
                    throw response.ErrorException;
                }

                if (response.StatusCode.IsSuccess())
                {
                    foreach (var plugin in response.Data)
                    {
                        yield return plugin;
                    }

                    if (response.Data.Count < MaxFilesPerPage)
                    {
                        yield break;
                    }

                    page++;
                }
                else
                {
                    yield break;
                }
            }
            while (true);
        }
    }
}
