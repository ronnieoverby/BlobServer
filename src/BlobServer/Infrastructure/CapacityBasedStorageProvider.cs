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
            _storages = storages.ToDictionary(x => x.Key);
            _mode = mode;
        }

        public enum SelectionMode
        {
            RandomlySelectStorageWithEnoughSpace,
            SelectStorageWithTheMostFreeSpace
        }

        public Task<IFileStorage> GetFileStorageAsync(UploadedFile uploadedFile)
        {
            switch (_mode)
            {
                case SelectionMode.RandomlySelectStorageWithEnoughSpace:
                    return RandomlySelectStorageWithEnoughSpaceAsync(uploadedFile);

                case SelectionMode.SelectStorageWithTheMostFreeSpace:
                    return SelectStorageWithTheMostFreeSpaceAsync(uploadedFile);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public IFileStorage GetStorageByKey(string key)
        {
            return _storages[key];
        }

        async private Task<IFileStorage> SelectStorageWithTheMostFreeSpaceAsync(UploadedFile uploadedFile)
        {
            return _storages.Values.OrderByDescending(async x => (await x.GetCapacityAsync()).Available.Bytes).First();
        }

        async private Task<IFileStorage> RandomlySelectStorageWithEnoughSpaceAsync(UploadedFile uploadedFile)
        {
            var storages = new List<IFileStorage>();

            foreach (var stg in _storages.Values)
            {
                var cap = await stg.GetCapacityAsync();

                if (uploadedFile.DataSize == null || cap.Available > uploadedFile.DataSize)
                    storages.Add(stg);
            }

            return storages.Any() ? storages.RandomElement() : _storages.Values.RandomElement();
        }
    }
}