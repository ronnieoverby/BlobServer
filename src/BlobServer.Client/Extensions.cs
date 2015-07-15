using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlobClient
{
    public static class Extensions
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

        private static readonly char[] DirSeparators = { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };

        public static string[] SplitPath(this string path)
        {
            if (path == null) throw new ArgumentNullException("path");
            var parts = path.Trim(DirSeparators).Split(DirSeparators);
            return parts;
        }

        public static string TrimDirectorySeparators(this string path)
        {
            return path.Trim(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }

        public static string EnsureEndsWith(this string s, string suffix)
        {
            return s.EndsWith(suffix) ? s : s + suffix;
        }
    }
}