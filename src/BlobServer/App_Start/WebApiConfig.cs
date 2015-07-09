using System.Web.Http;
using System.Web.Http.Hosting;
using BlobServer.Infrastructure;

namespace BlobServer
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var appConfig = Configuration.CreateUsingConfigR();
            config.DependencyResolver = new DependencyResolver(appConfig);

            // Web API configuration and services
            config.Services.Replace(typeof (IHostBufferPolicySelector), new CustomWebHostBufferPolicySelector());

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}