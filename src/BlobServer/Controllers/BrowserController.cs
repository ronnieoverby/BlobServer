using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using BlobServer.Infrastructure;
using BlobServer.Models;

namespace BlobServer.Controllers
{
    public class BrowserController : Controller
    {
        private readonly IStorageProvider _storageProvider;

        public BrowserController(IStorageProvider storageProvider)
        {
            _storageProvider = storageProvider;
        }


        private void ParsePath(string fullPath, out IFileStorage stg, out string localPath)
        {
            // todo set stg to null if key not found
            // so response can be 404

            var parts = fullPath.SplitPath();
            stg = _storageProvider.GetStorageByKey(parts[0]);
            localPath = string.Join("/", parts.Skip(1));
        }

        public async Task<ViewResult> Index(string path)
        {
            var model = new BrowserDirectoryModel {CurrentPath = path};

            if (string.IsNullOrWhiteSpace(path))
            {
                model.CurrentPath = "";
                model.Entries =
                    _storageProvider.Keys.Select(x => new Directory {FullName = x}).Cast<FileStorageEntry>().ToArray();
            }
            else
            {
                IFileStorage stg;
                string localpath;
                ParsePath(path, out stg, out localpath);

                var entries = await stg.GetEntriesAsync(localpath);
                model.Entries = entries;
            }

            return View(model);
        }
    }


    public class BrowserDirectoryModel
    {
        public string CurrentPath { get; set; }
        public FileStorageEntry[] Entries { get; set; }
    }
}