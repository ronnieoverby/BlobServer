﻿using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using BlobServer.Infrastructure;
using CoreTechs.Common;

namespace BlobServer.Controllers
{
    [RoutePrefix("api/files")]
    public class FileSystemController : ApiController
    {
        private readonly IStorageProvider _storages;
        private readonly IPathCreator _pathCreator;

        public FileSystemController()
        {
            _storages = new CapacityBasedStorageProvider(new[]
            {
                new FileSystemStorage(new DirectoryInfo("d:\\storagetest"), "D"),
                new FileSystemStorage(new DirectoryInfo("f:\\storagetest"), "F"),
            }, CapacityBasedStorageProvider.SelectionMode.RandomlySelectStorageWithEnoughSpace);

            _pathCreator = new DateTimePathCreator();
        }

        [Route("{*path}")]
        public async Task<HttpResponseMessage> Get(string path, string contentType = null)
        {
            IFileStorage stg;
            string localPath;
            ParsePath(path, out stg, out localPath);

            if (!await stg.ExistsAsync(localPath))
                return NotFound();

            var stream = await stg.GetReadStream(localPath);

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(stream)
            };

            contentType = contentType ?? MimeMapping.GetMimeMapping(Path.GetExtension(localPath));
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

            return response;
        }

        [Route]
        public async Task<HttpResponseMessage> Post([FromUri]string filename = null, [FromUri]string extension = null)
        {
            IFileStorage stg;
            string path;
            using (var stream = await Request.Content.ReadAsStreamAsync())
            {
                stg = await _storages.GetFileStorageAsync(ByteSize.FromBytes(stream.Length));
                path = _pathCreator.CreatePath(filename, extension);
                await stg.StoreAsync(path, stream);
            }
            var fullPath = string.Format("{0}/{1}", stg.Key, path);
            return new HttpResponseMessage
            {
                Content = new StringContent(fullPath)
            };
        }

        private new static HttpResponseMessage NotFound()
        {
            return new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent("Not Found")
            };
        }

        private void ParsePath(string fullPath, out IFileStorage stg, out string localPath)
        {
            var parts = fullPath.SplitPath();
            stg = _storages.GetStorageByKey(parts[0]);
            localPath = string.Join(Path.DirectorySeparatorChar.ToString(), parts.Skip(1));
        }
    }
}