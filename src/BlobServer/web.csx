using BlobServer.Infrastructure;
using using CoreTechs.Common;

var config = new Configuration
{
    StorageProvider = new CapacityBasedStorageProvider(new[]
            {
                new FileSystemStorage(new DirectoryInfo(@"C:\Users\roverby\Desktop\BlobServer\A"), "A"),
                new FileSystemStorage(new DirectoryInfo(@"C:\Users\roverby\Desktop\BlobServer\B"), "B"),
                new FileSystemStorage(new DirectoryInfo(@"C:\Users\roverby\Desktop\BlobServer\C"), "C"),
                new FileSystemStorage(new DirectoryInfo(@"\\neuron\Backups\test"), "neuron"),

            }, CapacityBasedStorageProvider.SelectionMode.RandomlySelectStorageWithEnoughSpace),

    PathCreator = new DateTimePathCreator{ Resolution = DateTimePrecision.Hour },
};

Add(config);