using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BlobClient;
using CoreTechs.Common;

namespace BlobServer.Infrastructure
{
    class MemoryStorage : IFileStorage
    {
        private readonly Dictionary<string, byte[]> _files = new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);

        public MemoryStorage(string uniqueKey)
        {
            if (uniqueKey == null) throw new ArgumentNullException("uniqueKey");

            Key = uniqueKey;
        }

        public string Key { get; private set; }
        public Task<FileStorageCapacity> GetCapacityAsync()
        {
            return Task.FromResult(new FileStorageCapacity
            {
                Available = new ByteSize(long.MaxValue),
                Total = new ByteSize(long.MaxValue)
            });
        }

        public async Task StoreAsync(string path, Stream stream)
        {
            using (var ms = new MemoryStream())
            {
                await stream.CopyToAsync(ms);
                _files[path] = ms.ToArray();
            }
        }

        public async Task AppendAsync(string path, Stream stream)
        {
            using (var ms = new MemoryStream())
            {
                await stream.CopyToAsync(ms);

                if (_files.ContainsKey(path))
                {
                    _files[path] = _files[path].Concat(ms.ToArray()).ToArray();
                }
                else
                {
                    _files[path] = ms.ToArray();
                }
            }
        }

        public Task<bool> DeleteAsync(string path)
        {
            return Task.FromResult(_files.Remove(path));
        }

        public Task<bool> ExistsAsync(string path)
        {
            return Task.FromResult(_files.ContainsKey(path));
        }

        public async Task<ByteSize> GetSize(string path)
        {
            if (await ExistsAsync(path))
            {
                return new ByteSize(_files[path].Length);
            }

            return null;
        }

        public Task<Stream> GetReadStream(string path)
        {
            return Task.FromResult((Stream) new MemoryStream(_files[path]));
        }

        public Task<FileStorageEntry[]> GetEntriesAsync(string path)
        {
            throw new NotImplementedException();
        }
    }
}