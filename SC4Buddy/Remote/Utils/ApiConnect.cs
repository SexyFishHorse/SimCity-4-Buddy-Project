namespace Nihei.SC4Buddy.Remote.Utils
{
    using System.Net.NetworkInformation;
    using Nihei.SC4Buddy.Configuration;
    using Nihei.SC4Buddy.Remote.Models;
    using RestSharp;

    public static class ApiConnect
    {

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

        public static bool HasConnectionAndIsFeatureEnabled(string feature)
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(Settings.Get(Settings.Keys.ApiBaseUrl)))
            {
                return false;
            }

            return Settings.Get<bool>(feature);
        }

        public static IRestClient GetClient()
        {
            return new RestClient(Settings.Get(Settings.Keys.ApiBaseUrl));
        }
    }
}
