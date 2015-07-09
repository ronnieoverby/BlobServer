using System.Threading.Tasks;
using BlobServer.Models;
using CoreTechs.Common;

namespace BlobServer.Infrastructure
{
    public interface IStorageProvider
    {
        Task<IFileStorage> GetFileStorageAsync(ByteSize requiredSize);
        IFileStorage GetStorageByKey(string key);
    }
}