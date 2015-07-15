using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using BlobClient;

namespace BlobServer.Controllers
{
    public class BrowserController : Controller
    {
        private readonly IStorageProvider _storageProvider;

        public BrowserController(IStorageProvider storageProvider)
        {
            _storageProvider = storageProvider;
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
                new PathParser(_storageProvider).Parse(path, out stg, out localpath);

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