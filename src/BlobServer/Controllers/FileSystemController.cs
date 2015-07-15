using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using BlobClient;
using BlobServer.Infrastructure;
using Humanizer;

namespace BlobServer.Controllers
{
    [ExceptionHandling]
    [RoutePrefix("api/files")]
    public class FileSystemController : ApiController
    {
        private readonly LocalBlobClient _client;

        public FileSystemController(IPathCreator pathCreator, IStorageProvider storages)
        {
            _client = new LocalBlobClient(storages, pathCreator);
        }

        [Route("{*path}")]
        public async Task<HttpResponseMessage> Get(string path, string contentType = null)
        {
            Stream stream = null;
            try
            {
                stream = await _client.GetStreamAsync(path);
            }
            catch (FileNotFoundException)
            {
                Respond(HttpStatusCode.NotFound);
            }

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(stream),
            };

            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = path.SplitPath().Last()
            };

            contentType = contentType ?? MimeMapping.GetMimeMapping(Path.GetExtension(path));
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
                var fullPath = await _client.StoreFromStreamAsync(stream, filename, extension, rootFolder);
                return Respond(HttpStatusCode.OK, fullPath);
            }
        }

        [Route("{*path}")]
        public async Task<HttpResponseMessage> Put([FromUri]string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                Respond(HttpStatusCode.BadRequest, "Path not specified");

            using (var stream = await Request.Content.ReadAsStreamAsync())
                await _client.StoreFromStreamAsync(stream, path: path);

            return Respond(HttpStatusCode.OK);
        }

        [Route("{*path}")]
        public async Task<HttpResponseMessage> Delete(string path)
        {
            try
            {
                await _client.DeleteAsync(path);
                return Respond(HttpStatusCode.NoContent);
            }
            catch (FileNotFoundException)
            {
                return Respond(HttpStatusCode.NotFound);
            }
        }

        [AcceptVerbs("PATCH")]
        [Route("{*path}")]
        public async Task<HttpResponseMessage> Patch(string path)
        {
            using (var stream = await Request.Content.ReadAsStreamAsync())
            {
                try
                {
                    await _client.AppendFromStreamAsync(path, stream);
                    return Respond(HttpStatusCode.NoContent);
                }
                catch (FileNotFoundException)
                {
                    return Respond(HttpStatusCode.NotFound);
                }
            }
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