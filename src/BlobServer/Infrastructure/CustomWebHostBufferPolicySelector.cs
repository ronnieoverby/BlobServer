using System;
using System.Web;
using System.Web.Http.WebHost;

namespace BlobServer.Infrastructure
{
    public class CustomWebHostBufferPolicySelector : WebHostBufferPolicySelector
    {
        public override bool UseBufferedInputStream(object hostContext)
        {
            var context = hostContext as HttpContextBase;
        
            if (context != null)
            {
                var controller = (string)context.Request.RequestContext.RouteData.Values["controller"];

                if (controller == null || string.Equals(controller, "FileSystem", StringComparison.OrdinalIgnoreCase))
                    return false;
            }

            return base.UseBufferedInputStream(hostContext);
        }
    }
}