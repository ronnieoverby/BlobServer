using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BlobServer.Models;
using BlobServer.Infrastructure;
using CoreTechs.Common;
using Directory = BlobServer.Models.Directory;
using File = BlobServer.Models.File;

namespace BlobServer.Infrastructure
{
    
    public class FileSystemStorage : IFileStorage
    {
        private readonly DirectoryInfo _directoryInfo;


        public FileSystemStorage(DirectoryInfo directoryInfo, string uniqueKey)
        {
            if (directoryInfo == null) throw new ArgumentNullException("directoryInfo");
            if (uniqueKey == null) throw new ArgumentNullException("uniqueKey");
            Key = uniqueKey;

            if (!directoryInfo.Exists)
                directoryInfo.Create();

            _directoryInfo = directoryInfo;
        }

        public string Key { get; private set; }

        public Task<FileStorageCapacity> GetCapacityAsync()
        {
            return Task.FromResult(FileStorageCapacity.Get(_directoryInfo.FullName));
        }

        public async Task StoreAsync(string path, Stream stream)
        {
            EnsurePathDoesntRepresentSubDirectory(path);

            var file = GetFileInfo(path);

            if (!file.Directory.Exists)
               file.Directory.Create();

            using (var dest = file.Open(FileMode.Create, FileAccess.Write, FileShare.None))
                await  stream.CopyToAsync(dest);
        }


        public async Task AppendAsync(string path, Stream stream)
        {
            EnsurePathDoesntRepresentSubDirectory(path);

            var file = GetFileInfo(path);

            if (!file.Directory.Exists)
                 file.Directory.Create();

            using (var dest = file.Open(FileMode.Append, FileAccess.Write, FileShare.None))
                await stream.CopyToAsync(dest);
        }

        private void EnsurePathDoesntRepresentSubDirectory(string path)
        {
            if (_directoryInfo.GetSubDirectory(path).Exists)
                throw new InvalidOperationException("Path represents an existing directory");
        }

        public Task<bool> DeleteAsync(string path)
        {
            var fileInfo = GetFileInfo(path);
            var result = false;

            if (fileInfo.Exists)
            {
                fileInfo.Delete();
                result = true;
            }

            return Task.FromResult(result);
        }

        public Task<bool> ExistsAsync(string path)
        {
            var file = _directoryInfo.GetFile(path);

            return Task.FromResult(file.Exists);

        }

        public Task<ByteSize> GetSize(string path)
        {
            var fileInfo = GetFileInfo(path);
            var size = ByteSize.FromBytes(fileInfo.Length);
            return Task.FromResult(size);
        }

        private FileInfo GetFileInfo(string path)
        {
            return _directoryInfo.GetFile(path);
        }

        public Task<Stream> GetReadStream(string path)
        {
            Stream stream = GetFileInfo(path).OpenRead();
            return Task.FromResult(stream);
        }

        public Task<FileStorageEntry[]> GetEntriesAsync(string path)
        {
            var d = _directoryInfo.GetSubDirectory(path);

            var entries = d.EnumerateDirectories().Select(dir => new Directory
            {
                FullName = new[]{Key,path.TrimDirectorySeparators(),dir.Name}.WhereNot(string.IsNullOrWhiteSpace).Join("/")
            }).Cast<FileStorageEntry>().ToList();

            entries.AddRange(d.EnumerateFiles().Select(f => new File
            {
                FullName = new[] { Key, path.TrimDirectorySeparators(), f.Name }.WhereNot(string.IsNullOrWhiteSpace).Join("/"),
                Length = f.Length
            }));

            return Task.FromResult(entries.ToArray());

        }
    }

}