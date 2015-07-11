using System.Web.Http;
using System.Web.Http.Hosting;
using BlobServer.Infrastructure;

namespace BlobServer
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config,DependencyResolver resolver)
        {
            config.DependencyResolver = resolver;

            // Web API configuration and services
            config.Services.Replace(typeof(IHostBufferPolicySelector), new CustomWebHostBufferPolicySelector());

            // Web API routes
            config.MapHttpAttributeRoutes();
/*
            config.Routes.MapHttpRoute(
                name: "FileSystem",
                routeTemplate: "api/files/{*path}",
                defaults: new { path = RouteParameter.Optional, controller = "FileSystem" }
            );*/

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}