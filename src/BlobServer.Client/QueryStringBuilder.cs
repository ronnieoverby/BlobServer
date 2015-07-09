using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace BlobServer.Client
{
    public class QueryStringBuilder : Dictionary<string,string>
    {
        public override string ToString()
        {
            return "?" +
                   string.Join("&",
                       from pair in this
                       let val = WebUtility.UrlEncode(pair.Value ?? "")
                       select pair.Key + "=" + val
                       );
        }
    }
}