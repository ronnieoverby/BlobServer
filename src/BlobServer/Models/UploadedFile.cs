using System.IO;
using CoreTechs.Common;

namespace BlobServer.Models
{
    public class UploadedFile
    {
        public Stream DataStream { get; set; }
        public string Filename { get; set; }
        public string Extension { get; set; }
        public ByteSize DataSize { get; set; }
    }
}