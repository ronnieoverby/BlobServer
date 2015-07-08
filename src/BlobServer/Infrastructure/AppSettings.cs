using System.IO;
using System.Linq;

namespace BlobServer.Infrastructure
{
    public class AppSettings : CoreTechs.Common.AppSettings
    {
        public DirectoryInfo[] StorageRoots
        {
            get
            {
                var roots = GetRequiredSetting(ParseStorageRoots);

                foreach (var r in roots.Where(r => !r.Exists))
                    r.Create();

                return roots;
            }
        }

        public DirectoryInfo[] ParseStorageRoots(string setting)
        {
            return setting.Split(',').Select(p => new DirectoryInfo(p.Trim())).ToArray();
        }
    }
}