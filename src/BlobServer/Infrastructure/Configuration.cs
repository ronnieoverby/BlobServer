using ConfigR;

namespace BlobServer.Infrastructure
{
    public class Configuration
    {
        public IStorageProvider StorageProvider { get; set; }
        public IPathCreator PathCreator { get; set; }

        public static Configuration CreateUsingConfigR()
        {
            var assembly = typeof(Configuration).Assembly; 
            Config.GlobalAutoLoadingReferences.Add(assembly);
            var config = Config.Global.Get<Configuration>();
            return config;
        }
    }
}

