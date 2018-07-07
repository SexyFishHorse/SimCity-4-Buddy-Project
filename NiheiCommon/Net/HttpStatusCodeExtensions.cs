namespace Nihei.Common.Net
{
    using System.Net;

    public static class HttpStatusCodeExtensions
    {
        public static bool IsSuccess(this HttpStatusCode statusCode)
        {
            return
                statusCode == HttpStatusCode.OK ||
                statusCode == HttpStatusCode.Created ||
                statusCode == HttpStatusCode.Accepted ||
                statusCode == HttpStatusCode.NonAuthoritativeInformation ||
                statusCode == HttpStatusCode.ResetContent ||
                statusCode == HttpStatusCode.PartialContent;
        }
    }
}
