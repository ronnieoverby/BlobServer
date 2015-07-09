using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlobServer.Client
{
    public class BlobServerClient : IDisposable
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

        public async Task<string> StoreBytesAsync(byte[] bytes, string filename = null, string extension = null, string rootFolder=null)
        {
            if (bytes == null) throw new ArgumentNullException("bytes");
            using (var ms = new MemoryStream(bytes))
                return await StoreFromStreamAsync(ms, filename, extension).ConfigureAwait(false);
        }

        public async Task<string> StoreFromStreamAsync(Stream stream, string filename = null, string extension = null, string rootFolder = null)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            var qs = new QueryStringBuilder();

            if (!string.IsNullOrWhiteSpace(filename))
                qs["filename"] = filename;

            if (!string.IsNullOrWhiteSpace(extension))
                qs["extension"] = extension;

            if (!string.IsNullOrWhiteSpace(rootFolder))
                qs["rootFolder"] = rootFolder;

            var resp = await _client.PutAsync(qs.ToString(), new StreamContent(stream)).ConfigureAwait(false);

            var path = await resp.EnsureSuccessStatusCode().Content.ReadAsStringAsync().ConfigureAwait(false);
            return path;
        }

        public async Task<byte[]> GetBytesAsync(string path)
        {
            if (path == null) throw new ArgumentNullException("path");
            using (var stream = await GetStreamAsync(path).ConfigureAwait(false))
            using (var ms = new MemoryStream())
            {
                await stream.CopyToAsync(ms).ConfigureAwait(false);
                return ms.ToArray();
            }
        }

        public async Task<Stream> GetStreamAsync(string path)
        {
            if (path == null) throw new ArgumentNullException("path");

            var qs = new QueryStringBuilder {{"path", path}};

            var resp = await _client.GetAsync(qs.ToString()).ConfigureAwait(false);
            return await resp.EnsureSuccessStatusCode().Content.ReadAsStreamAsync().ConfigureAwait(false);
        }

        public async Task DeleteAsync(string path)
        {
            if (path == null) throw new ArgumentNullException("path");
            var qs = new QueryStringBuilder { { "path", path } };
            var resp = await _client.DeleteAsync(qs.ToString()).ConfigureAwait(false);
            resp.EnsureSuccessStatusCode();
        }

        public async Task AppendBytesAsync(string path, byte[] bytes)
        {
            if (path == null) throw new ArgumentNullException("path");
            if (bytes == null) throw new ArgumentNullException("bytes");

            using (var ms = new MemoryStream(bytes))
                await AppendFromStreamAsync(path, ms).ConfigureAwait(false);
        }

        public async Task AppendFromStreamAsync(string path, Stream stream)
        {
            if (path == null) throw new ArgumentNullException("path");
            if (stream == null) throw new ArgumentNullException("stream");

            var content = new StreamContent(stream);
            var qs = new QueryStringBuilder { { "path", path } };
            var resp = await _client.SendAsync("PATCH", qs.ToString(), content).ConfigureAwait(false);
            resp.EnsureSuccessStatusCode();
        }
    }
}