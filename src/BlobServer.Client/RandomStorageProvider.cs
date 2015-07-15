using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlobClient;
using CoreTechs.Common;

namespace BlobServer.Infrastructure
{
    public class RandomStorageProvider : IStorageProvider
    {
        private readonly Dictionary<string, IFileStorage> _storages;

        public RandomStorageProvider(IEnumerable<IFileStorage> storages)
        {
            if (storages == null) throw new ArgumentNullException("storages");
            _storages = storages.ToDictionary(x => x.Key, StringComparer.OrdinalIgnoreCase);
        }

        public Task<IFileStorage> GetFileStorageAsync(ByteSize requiredSize)
        {
            return Task.FromResult(_storages.Values.RandomElement());
        }

        public IFileStorage GetStorageByKey(string key)
        {
            return _storages[key];
        }

        public IEnumerable<string> Keys
        {
            get { return _storages.Keys; }
        }

        public IEnumerable<IFileStorage> Storages { get { return _storages.Values; } }
    }
}