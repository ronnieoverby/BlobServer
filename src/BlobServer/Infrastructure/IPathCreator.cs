namespace BlobServer.Infrastructure
{
    public interface IPathCreator
    {
        string CreatePath(string fileName = null, string extension = null);
    }
}