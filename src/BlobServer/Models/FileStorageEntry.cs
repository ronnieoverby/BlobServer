using System.Linq;
using BlobServer.Infrastructure;

namespace BlobServer.Models
{
    public abstract class FileStorageEntry
    {
        public string FullName { get; set; }

        public string Name {
            get { return FullName.SplitPath().Last(); }
        } 
    }
}