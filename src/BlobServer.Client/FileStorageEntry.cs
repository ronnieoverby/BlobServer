using System.Linq;

namespace BlobClient
{
    public abstract class FileStorageEntry
    {
        public string FullName { get; set; }

        public string Name {
            get { return FullName.SplitPath().Last(); }
        } 
    }
}