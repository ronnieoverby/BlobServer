using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlobServer.Client
{
    static class Extensions
    {
        public static Task<HttpResponseMessage> SendAsync(this HttpClient client, string httpMethod, string url, HttpContent content = null)
        {
            if (client == null) throw new ArgumentNullException("client");
            if (httpMethod == null) throw new ArgumentNullException("httpMethod");
            if (url == null) throw new ArgumentNullException("url");

            var method = new HttpMethod(httpMethod);

            var request = new HttpRequestMessage(method, url)
            {
                Content = content
            };

            return client.SendAsync(request);
        }
    }
}