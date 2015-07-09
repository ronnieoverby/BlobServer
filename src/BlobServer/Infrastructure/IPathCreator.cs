namespace BlobServer.Infrastructure
{
    public interface IPathCreator
    {
        string CreatePath(string rootFolder = null, string fileName = null, string extension = null);
        string AppendRandomDirectory(string path);
    }
}