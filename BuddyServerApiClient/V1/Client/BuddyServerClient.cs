namespace Asser.Sc4Buddy.Server.Api.V1.Client
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using Asser.Sc4Buddy.Server.Api.V1.Models;
    using RestSharp;

    public class BuddyServerClient : IBuddyServerClient
    {
        private const int MaxFilesPerPage = 100;

        private readonly IRestClient client;

        public BuddyServerClient(IRestClient client)
        {
            this.client = client;
        }

        public IEnumerable<File> GetAllFiles() => GetAllItems<File>("files");

        public IEnumerable<Plugin> GetAllPlugins() => GetAllItems<Plugin>("plugins");

        public Plugin GetPlugin(Guid pluginId)
        {
            var request = new RestRequest("plugins/{pluginId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddUrlSegment("pluginId", pluginId.ToString());

            var response = client.Get<Plugin>(request);
            if (response.IsSuccessful == false && response.ErrorException != null)
            {
                throw response.ErrorException;
            }

            return response.Data;
        }

        private IEnumerable<T> GetAllItems<T>(string method)
        {
            var page = 1;
            do
            {
                var request = new RestRequest(method, Method.GET) { RequestFormat = DataFormat.Json };
                request.AddQueryParameter("page", page + string.Empty);
                request.AddQueryParameter("perPage", MaxFilesPerPage + string.Empty);

                var response = client.Get<List<T>>(request);

                if (response.IsSuccessful == false)
                {
                    if (response.ErrorException != null)
                    {
                        if (response.ErrorException is WebException webEx
                            && webEx.Status == WebExceptionStatus.NameResolutionFailure)
                        {
                            yield break;
                        }

                        throw response.ErrorException;
                    }
                }

                foreach (var item in response.Data)
                {
                    yield return item;
                }

                if (response.Data.Count < MaxFilesPerPage)
                {
                    yield break;
                }

                page++;
            }
            while (true);
        }
    }
}
