namespace NIHEI.SC4Buddy.Remote.Utils
{
    using System.Net.NetworkInformation;
    using NIHEI.SC4Buddy.Configuration;
    using NIHEI.SC4Buddy.Remote.Models;
    using RestSharp;

    public class ApiConnect
    {
        private readonly string baseUrl;

        public ApiConnect(string baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        public static void ThrowErrorOnConnectionOrDisabledFeature(string feature)
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                throw new BuddyServerException("No internet connection available.", ApiConnectCodes.NoNetworkConnection);
            }

            if (string.IsNullOrWhiteSpace(Settings.Get(Settings.Keys.ApiBaseUrl)))
            {
                throw new BuddyServerException("Api base url is not defined.", ApiConnectCodes.NoBaseApiDefined);
            }

            if (!Settings.Get<bool>(feature))
            {
                throw new BuddyServerException("The feature " + feature + " is disabled.", ApiConnectCodes.FeatureDisabled);
            }
        }

        public IRestClient GetClient()
        {
            return new RestClient(baseUrl);
        }
    }
}
