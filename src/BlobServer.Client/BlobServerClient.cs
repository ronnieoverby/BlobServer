using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlobClient
{
    public class BlobServerClient : IBlobClient
    {
        private readonly HttpClient _client;

        public BlobServerClient(string baseUrl)
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(new Uri(baseUrl), new Uri("api/files/", UriKind.Relative))
            };
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        public async Task<string> StoreFromStreamAsync(Stream stream, string filename = null, string extension = null, string rootFolder = null, string path = null)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            var qs = new QueryStringBuilder();

            if (string.IsNullOrWhiteSpace(path))
            {

                if (!string.IsNullOrWhiteSpace(filename))
                    qs["filename"] = filename;

                if (!string.IsNullOrWhiteSpace(extension))
                    qs["extension"] = extension;

                if (!string.IsNullOrWhiteSpace(rootFolder))
                    qs["rootFolder"] = rootFolder;

                var resp = await _client.PostAsync(qs.ToString(), new StreamContent(stream)).ConfigureAwait(false);

                await EnsureSuccess(resp);

                path = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
            else
            {
                qs["path"] = path;
                var resp = await _client.PutAsync(qs.ToString(), new StreamContent(stream)).ConfigureAwait(false);
                await EnsureSuccess(resp);
              
                path = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
            return path;
        }

        private static async Task EnsureSuccess(HttpResponseMessage resp)
        {
            if (resp.IsSuccessStatusCode)
                return;

            var ex = new BlobServerRequestException
            {
                StatusCode = resp.StatusCode,
                Headers = resp.Headers,
                
            };

            var stream = await resp.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            ex.Content = ms.ToArray();

            if (resp.Content.Headers.ContentType.MediaType.StartsWith("text",StringComparison.OrdinalIgnoreCase))
            {
                ms.Position = 0;
                var reader = new StreamReader(ms);
                ex.StringContent = reader.ReadToEnd();
            }


            throw ex;
        }

        public async Task<Stream> GetStreamAsync(string path)
        {
            if (path == null) throw new ArgumentNullException("path");

            var qs = new QueryStringBuilder { { "path", path } };

            var resp = await _client.GetAsync(qs.ToString()).ConfigureAwait(false);
            await EnsureSuccess(resp);

            return await resp.Content.ReadAsStreamAsync().ConfigureAwait(false);
        }

        public async Task DeleteAsync(string path)
        {
            if (path == null) throw new ArgumentNullException("path");
            var qs = new QueryStringBuilder { { "path", path } };
            var resp = await _client.DeleteAsync(qs.ToString()).ConfigureAwait(false);
            await EnsureSuccess(resp);
        }

        public async Task AppendFromStreamAsync(string path, Stream stream)
        {
            if (path == null) throw new ArgumentNullException("path");
            if (stream == null) throw new ArgumentNullException("stream");

            var content = new StreamContent(stream);
            var qs = new QueryStringBuilder { { "path", path } };
            var resp = await _client.SendAsync("PATCH", qs.ToString(), content).ConfigureAwait(false);
            await EnsureSuccess(resp);
        }
    }
}