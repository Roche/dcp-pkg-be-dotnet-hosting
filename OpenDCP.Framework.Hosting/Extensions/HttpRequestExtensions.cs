using Microsoft.AspNetCore.Http;

namespace OpenDCP.Framework.Hosting.Extensions
{
    public static class HttpRequestExtensions
    {
        public static string GetBaseUrl(this HttpRequest httpRequest)
        {
            string baseUrl = httpRequest.Scheme + "://" + httpRequest.Host;
            return baseUrl;
        }

    }
}
