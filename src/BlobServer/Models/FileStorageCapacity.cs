using System.Runtime.InteropServices;
using CoreTechs.Common;

namespace BlobServer.Models
{
    public class FileStorageCapacity
    {
        public ByteSize Total { get; set; }
        public ByteSize Available { get; set; }

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetDiskFreeSpaceEx(string lpDirectoryName,
           out ulong lpFreeBytesAvailable,
           out ulong lpTotalNumberOfBytes,
           out ulong lpTotalNumberOfFreeBytes);

        public static FileStorageCapacity Get(string path)
        {
            ulong freeBytesAvailable;
            ulong totalCapacity;
            ulong totalFreeBytes;

            return GetDiskFreeSpaceEx(path, out freeBytesAvailable, out totalCapacity, out totalFreeBytes)
                ? new FileStorageCapacity
                {
                    Available = ByteSize.FromBytes((long) freeBytesAvailable),
                    Total = ByteSize.FromBytes((long) totalCapacity)
                }
                : null;
        }
    }
}