using System.Reflection;
using BlobClient;
using ConfigR;
using CoreTechs.Common;

namespace BlobServer.Infrastructure
{
    public class Configuration
    {
        public IStorageProvider StorageProvider { get; set; }
        public IPathCreator PathCreator { get; set; }

        public static Configuration CreateUsingConfigR()
        {
            Config.GlobalAutoLoadingReferences.Add(typeof(IStorageProvider).Assembly);
            Config.GlobalAutoLoadingReferences.Add(typeof(Configuration).Assembly);
            Config.GlobalAutoLoadingReferences.Add(typeof (DateTimePrecision).Assembly);

            var config = Config.Global.Get<Configuration>();
            return config;
        }
    }
}

