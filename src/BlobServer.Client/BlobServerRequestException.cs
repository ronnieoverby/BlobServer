using System;
using System.Net;
using System.Net.Http.Headers;

namespace BlobClient
{
    [Serializable]
    public class BlobServerRequestException : Exception
    {

        public HttpStatusCode StatusCode { get; internal set; }
        public HttpResponseHeaders Headers { get; internal set; }
        public byte[] Content { get; internal set; }
        public string StringContent { get; internal set; }
    }
}