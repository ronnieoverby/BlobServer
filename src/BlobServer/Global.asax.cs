using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using BlobServer.Infrastructure;
using DependencyResolver = System.Web.Mvc.DependencyResolver;

namespace BlobServer
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            var config = Configuration.CreateUsingConfigR();
            var resolver = new Infrastructure.DependencyResolver(config);
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(cfg => WebApiConfig.Register(cfg, resolver));
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);


            DependencyResolver.SetResolver(resolver);
        }
    }
}
