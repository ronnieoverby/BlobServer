using BlobClient;
using NUnit.Framework;

namespace Tests
{
    public class ClientTests
    {
        private const string BaseUrl = "http://localhost:63238";

        [Test,Explicit]
        public async void CanPostAndGetFile()
        {
            using (var client = new BlobServerClient(BaseUrl))
            {


                var bytes = await client.GetBytesAsync("A/xyz.txt");
            }
        }
    }
}
