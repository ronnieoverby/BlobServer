using System;
using System.IO;
using System.Threading.Tasks;

namespace BlobClient
{
    public static class BlobClientExtensions
    {
        public static async Task<string> StoreBytesAsync(this IBlobClient client, byte[] bytes, string filename = null,
            string extension = null, string rootFolder = null, string path = null)
        {
            if (bytes == null) throw new ArgumentNullException("bytes");
            using (var ms = new MemoryStream(bytes))
                return await client.StoreFromStreamAsync(ms, filename, extension, path).ConfigureAwait(false);
        }

        public static async Task<byte[]> GetBytesAsync( this IBlobClient client, string path)
        {
            if (path == null) throw new ArgumentNullException("path");
            using (var stream = await client.GetStreamAsync(path).ConfigureAwait(false))
            using (var ms = new MemoryStream())
            {
                await stream.CopyToAsync(ms).ConfigureAwait(false);
                return ms.ToArray();
            }
        }

        public static async Task AppendBytesAsync(this IBlobClient client, string path, byte[] bytes)
        {
            if (path == null) throw new ArgumentNullException("path");
            if (bytes == null) throw new ArgumentNullException("bytes");

            using (var ms = new MemoryStream(bytes))
                await client.AppendFromStreamAsync(path, ms).ConfigureAwait(false);
        }
    }
}