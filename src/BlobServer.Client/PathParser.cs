using System;
using System.Linq;

namespace BlobClient
{
    public class PathParser
    {
        private readonly IStorageProvider _storageProvider;

        public PathParser(IStorageProvider storageProvider)
        {
            if (storageProvider == null) throw new ArgumentNullException("storageProvider");
            _storageProvider = storageProvider;
        }

        public void Parse(string fullPath, out IFileStorage stg, out string localPath)
        {
            var parts = fullPath.SplitPath();
            stg = _storageProvider.GetStorageByKey(parts[0]);
            localPath = string.Join("/", parts.Skip(1));
        }
    }
}