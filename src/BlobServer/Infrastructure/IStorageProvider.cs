using System.Threading.Tasks;
using BlobServer.Models;

namespace BlobServer.Infrastructure
{
    public interface IStorageProvider
    {
        Task<IFileStorage> GetFileStorageAsync(UploadedFile uploadedFile);
        IFileStorage GetStorageByKey(string key);
    }
}