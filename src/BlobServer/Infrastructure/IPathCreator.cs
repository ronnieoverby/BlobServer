namespace BlobServer.Infrastructure
{
    internal interface IPathCreator
    {
        string CreatePath(string fileName = null, string extension = null);
    }
}