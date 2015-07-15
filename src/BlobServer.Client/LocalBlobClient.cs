using System;
using System.IO;
using System.Threading.Tasks;
using CoreTechs.Common;

namespace BlobClient
{
    public class LocalBlobClient : IBlobClient
    {
        private readonly IStorageProvider _storageProvider;
        private readonly IPathCreator _pathCreator;

        public LocalBlobClient(IStorageProvider storageProvider, IPathCreator pathCreator)
        {
            if (storageProvider == null) throw new ArgumentNullException("storageProvider");
            if (pathCreator == null) throw new ArgumentNullException("pathCreator");

            _storageProvider = storageProvider;
            _pathCreator = pathCreator;
        }

        public async Task<string> StoreFromStreamAsync(Stream stream, string filename = null, string extension = null, string rootFolder = null,
            string path = null)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            if (path.IsNullOrWhiteSpace())
            {
                var stg = await _storageProvider.GetFileStorageAsync(ByteSize.FromBytes(stream.Length)).ConfigureAwait(false);

                 path = _pathCreator.CreatePath((rootFolder ?? "").TrimDirectorySeparators(), filename, extension)
                    .TrimDirectorySeparators();

                 while (await stg.ExistsAsync(path).ConfigureAwait(false))
                {
                    // file exists at path
                    // do not overwrite

                    path = _pathCreator.AppendRandomDirectory(path);
                }

                 await stg.StoreAsync(path, stream).ConfigureAwait(false);

                var fullPath = string.Format("{0}/{1}", stg.Key, path);
                return fullPath;
            }
            else
            {
                IFileStorage stg;
                string localPath;
                new PathParser(_storageProvider).Parse(path, out stg, out localPath);
                await stg.StoreAsync(path, stream).ConfigureAwait(false);
                return path;
            }

        }

        public async Task<Stream> GetStreamAsync(string path)
        {
            IFileStorage stg;
            string localPath;
            new PathParser(_storageProvider).Parse(path, out stg, out localPath);

            if (!await stg.ExistsAsync(localPath).ConfigureAwait(false))
                throw new FileNotFoundException(path);

            var stream = await stg.GetReadStream(localPath).ConfigureAwait(false);
            return stream;
        }

        public async Task DeleteAsync(string path)
        {
            IFileStorage stg;
            string localPath;
            new PathParser(_storageProvider).Parse(path, out stg, out localPath);

            if (!await stg.ExistsAsync(localPath).ConfigureAwait(false))
                throw new FileNotFoundException(path);

            await stg.DeleteAsync(localPath).ConfigureAwait(false);
        }

        public async Task AppendFromStreamAsync(string path, Stream stream)
        {
            IFileStorage stg;
            string localPath;
            new PathParser(_storageProvider).Parse(path, out stg, out localPath);

            if (!await stg.ExistsAsync(localPath).ConfigureAwait(false))
                throw new FileNotFoundException(path);
            await stg.AppendAsync(localPath, stream).ConfigureAwait(false);
        }

        public void Dispose()
        {
        }
    }
}