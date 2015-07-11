using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlobServer.Models;
using CoreTechs.Common;

namespace BlobServer.Infrastructure
{
    public class CapacityBasedStorageProvider : IStorageProvider
    {
        private readonly SelectionMode _mode;
        private readonly Dictionary<string, IFileStorage> _storages;

        public CapacityBasedStorageProvider(IEnumerable<IFileStorage> storages, SelectionMode mode)
        {
            if (storages == null) throw new ArgumentNullException("storages");
            _storages = storages.ToDictionary(x => x.Key, StringComparer.OrdinalIgnoreCase);
            _mode = mode;
        }

        public enum SelectionMode
        {
            RandomlySelectStorageWithEnoughSpace,
            SelectStorageWithTheMostFreeSpace
        }

        public Task<IFileStorage> GetFileStorageAsync(ByteSize requiredSize)
        {
            switch (_mode)
            {
                case SelectionMode.RandomlySelectStorageWithEnoughSpace:
                    return RandomlySelectStorageWithEnoughSpaceAsync(requiredSize);

                case SelectionMode.SelectStorageWithTheMostFreeSpace:
                    return SelectStorageWithTheMostFreeSpaceAsync();

                default:
                    throw new ArgumentOutOfRangeException();
            }
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

        async private Task<IFileStorage> SelectStorageWithTheMostFreeSpaceAsync()
        {
            var stats = new List<Tuple<IFileStorage, FileStorageCapacity>>();

            foreach (var stg in _storages.Values)
            {
                var cap = await stg.GetCapacityAsync();
                stats.Add(Tuple.Create(stg, cap));
            }

            return stats.OrderByDescending(x => x.Item2.Available).First().Item1;
        }

        async private Task<IFileStorage> RandomlySelectStorageWithEnoughSpaceAsync(ByteSize requiredSize)
        {
            var storages = new List<IFileStorage>();

            foreach (var stg in _storages.Values)
            {
                var cap = await stg.GetCapacityAsync();

                if (requiredSize == null || cap.Available > requiredSize)
                    storages.Add(stg);
            }

            return storages.Any() ? storages.RandomElement() : _storages.Values.RandomElement();
        }
    }
}