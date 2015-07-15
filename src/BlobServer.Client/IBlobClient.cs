using System;
using System.IO;
using System.Threading.Tasks;

namespace BlobClient
{
    public interface IBlobClient : IDisposable
    {
        Task<string> StoreFromStreamAsync(Stream stream, string filename = null, string extension = null, string rootFolder = null, string path = null);
        Task<Stream> GetStreamAsync(string path);
        Task DeleteAsync(string path);
        Task AppendFromStreamAsync(string path, Stream stream);
    }
}