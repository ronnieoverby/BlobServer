using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using BlobServer.Infrastructure;

namespace BlobServer.Controllers
{
    [RoutePrefix("api/files")]
    public class FileSystemController : ApiController
    {
        private readonly IStorageProvider _storages;

        public FileSystemController()
        {
            _storages = new RandomStorageProvider(new[]
            {
                new FileSystemStorage(new DirectoryInfo("d:\\storagetest"), "D_DRIVE"),
                new FileSystemStorage(new DirectoryInfo("f:\\storagetest"), "F_DRIVE"),
            });
        }

        [Route("{*path}")]
        public async Task<HttpResponseMessage> Get(string path, string contentType = null)
        {
            IFileStorage stg;
            string localPath;
            ParsePath(path, out stg, out localPath);

            if (!await stg.ExistsAsync(localPath))
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            var stream = await stg.GetReadStream(localPath);

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(stream)
            };

            contentType = contentType ?? MimeMapping.GetMimeMapping(Path.GetExtension(localPath));
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

            return response;
        }

        private void ParsePath(string fullPath, out IFileStorage stg, out string localPath)
        {
            var parts = fullPath.SplitPath();
            stg = _storages.GetStorageByKey(parts[0]);
            localPath = string.Join(Path.DirectorySeparatorChar.ToString(), parts.Skip(1));
        }
    }
}