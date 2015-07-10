using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using BlobServer.Infrastructure;
using CoreTechs.Common;
using Humanizer;

namespace BlobServer.Controllers
{
    [ExceptionHandling]
    [RoutePrefix("api/files")]
    public class FileSystemController : ApiController
    {
        private readonly IStorageProvider _storages;
        private readonly IPathCreator _pathCreator;

        public FileSystemController(IPathCreator pathCreator, IStorageProvider storages)
        {
            _pathCreator = pathCreator;
            _storages = storages;
        }

        [Route("{*path}")]
        public async Task<HttpResponseMessage> Get(string path, string contentType = null)
        {
            IFileStorage stg;
            string localPath;
            ParsePath(path, out stg, out localPath);

            if (!await stg.ExistsAsync(localPath))
                return Respond(HttpStatusCode.NotFound);

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
        public async Task<HttpResponseMessage> Post(
            [FromUri]string filename = null,
            [FromUri]string extension = null,
            [FromUri]string rootFolder = null)
        {
            using (var stream = await Request.Content.ReadAsStreamAsync())
            {
                var stg = await _storages.GetFileStorageAsync(ByteSize.FromBytes(stream.Length));

                var path = _pathCreator.CreatePath(rootFolder.TrimDirectorySeparators(), filename, extension)
                    .TrimDirectorySeparators();

                while (await stg.ExistsAsync(path))
                {
                    // file exists at path
                    // do not overwrite

                    path = _pathCreator.AppendRandomDirectory(path);
                }

                await stg.StoreAsync(path, stream);

                var fullPath = string.Format("{0}/{1}", stg.Key, path);
                return Respond(HttpStatusCode.OK, fullPath);
            }
        }

        [Route("{*path}")]
        public async Task<HttpResponseMessage> Put([FromUri]string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                Respond(HttpStatusCode.BadRequest, "Path not specified");

            IFileStorage stg;
            string localPath;
            ParsePath(path, out stg, out localPath);

            using (var stream = await Request.Content.ReadAsStreamAsync())
                await stg.StoreAsync(path, stream);

            return Respond(HttpStatusCode.OK);
        }

        [Route("{*path}")]
        public async Task<HttpResponseMessage> Delete(string path)
        {
            IFileStorage stg;
            string localPath;
            ParsePath(path, out stg, out localPath);

            if (!await stg.ExistsAsync(localPath))
                return Respond(HttpStatusCode.NotFound);

            await stg.DeleteAsync(localPath);
            return Respond(HttpStatusCode.NoContent);
        }

        [AcceptVerbs("PATCH")]
        [Route("{*path}")]
        public async Task<HttpResponseMessage> Patch(string path)
        {
            IFileStorage stg;
            string localPath;
            ParsePath(path, out stg, out localPath);

            if (!await stg.ExistsAsync(localPath))
                return Respond(HttpStatusCode.NotFound);

            using (var stream = await Request.Content.ReadAsStreamAsync())
            {
                await stg.AppendAsync(localPath, stream);
                return Respond(HttpStatusCode.NoContent);
            }
        }

        private void ParsePath(string fullPath, out IFileStorage stg, out string localPath)
        {
            // todo set stg to null if key not found
            // so response can be 404

            var parts = fullPath.SplitPath();
            stg = _storages.GetStorageByKey(parts[0]);
            localPath = string.Join("/", parts.Skip(1));
        }

        private static HttpResponseMessage Respond(HttpStatusCode statusCode, string text = null)
        {
            if (statusCode == HttpStatusCode.NoContent)
            {
                if (!string.IsNullOrEmpty(text))
                    throw new InvalidOperationException(
                        "204 NoContent means no content! You tried to respond with: " + text);

                text = string.Empty;
            }

            return new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(text ?? statusCode.Humanize())
            };
        }
    }
}