using System.Collections.Generic;
using System.Threading.Tasks;
using CoreTechs.Common;

namespace BlobClient
{
    public interface IStorageProvider
    {
        Task<IFileStorage> GetFileStorageAsync(ByteSize requiredSize);
        IFileStorage GetStorageByKey(string key);
        IEnumerable<string> Keys { get; }
        IEnumerable<IFileStorage> Storages { get; }
    }
}