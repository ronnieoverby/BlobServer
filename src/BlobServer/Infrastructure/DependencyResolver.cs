using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Dependencies;
using BlobServer.Controllers;

namespace BlobServer.Infrastructure
{
    public class DependencyResolver : IDependencyResolver, System.Web.Mvc.IDependencyResolver
    {
        private readonly Configuration _configuration;

        public DependencyResolver(Configuration configuration)
        {
            _configuration = configuration;
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof (FileSystemController))
                return new FileSystemController(_configuration.PathCreator, _configuration.StorageProvider);

            if (serviceType == typeof (BrowserController))
                return new BrowserController(_configuration.StorageProvider);

            return null;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return Enumerable.Empty<object>();
        }

        public IDependencyScope BeginScope()
        {
            return this;
        }

        public void Dispose()
        {

        }
    }
}