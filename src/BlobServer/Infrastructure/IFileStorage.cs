using System.IO;
using System.Threading.Tasks;
using BlobServer.Models;
using CoreTechs.Common;

namespace BlobServer.Infrastructure
{
    public interface IFileStorage
    {
        string Key { get; }
        Task<FileStorageCapacity> GetCapacityAsync();
        Task StoreAsync(string path, Stream stream);
        Task AppendAsync(string path, Stream stream);
        Task<bool> DeleteAsync(string path);
        Task<bool> ExistsAsync(string path);
        Task<ByteSize> GetSize(string path);
        Task<Stream> GetReadStream(string path);
    }
}

