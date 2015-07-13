<Query Kind="Program">
  <Reference Relative="BlobServer.Client\bin\Debug\BlobServer.Client.dll">D:\Code\BlobServer\src\BlobServer.Client\bin\Debug\BlobServer.Client.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.Http.dll</Reference>
  <NuGetReference>CoreTechs.Common</NuGetReference>
  <NuGetReference>Microsoft.AspNet.WebApi.Client</NuGetReference>
  <Namespace>BlobServer.Client</Namespace>
  <Namespace>CoreTechs.Common</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

async Task Main()
{
	using(var client = new BlobServerClient("http://blobs.ctx.local"))
	{
		var files = new DirectoryInfo(@"C:\Users\roverby\Desktop").GetFiles("en_visual_studio_professional_2012_x86_dvd_920779.iso",SearchOption.AllDirectories);
		while(true)
		{
			var file = files.RandomElement();		
			client.StoreBytesAsync(
			var path = await client.StoreFromStreamAsync(
				new SlowStream(file.OpenRead(),() => (RNG.NextDouble() * 1) .Seconds().Dump()), file.Name, rootFolder: "Batch/5");
			var bytes = await client.GetBytesAsync(path.Dump());
			await client.AppendBytesAsync(path, new byte[]{1});
			await client.DeleteAsync(path);
		}
	}
}

class SlowStream : Stream
{
    private readonly Stream _stream;
    private readonly Func<TimeSpan> _delayFunc;

    public SlowStream(Stream stream, Func<TimeSpan> delayFunc)
    {
        _stream = stream;
        _delayFunc = delayFunc;
    }

    public override void Flush()
    {
        Thread.Sleep(_delayFunc());
        _stream.Flush();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        Thread.Sleep(_delayFunc());
        return _stream.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
        Thread.Sleep(_delayFunc());
        _stream.SetLength(value);
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        Thread.Sleep(_delayFunc());
        return _stream.Read(buffer, offset, count.Dump());
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        Thread.Sleep(_delayFunc());
        _stream.Write(buffer, offset, count);
    }

    public override bool CanRead
    {
        get { return _stream.CanRead; }
    }

    public override bool CanSeek
    {
        get { return _stream.CanSeek; }
    }

    public override bool CanWrite
    {
        get { return _stream.CanWrite; }
    }

    public override long Length
    {
        get { return _stream.Length; }
    }

    public override long Position
    {
        get { return _stream.Position; }
        set
        {
            Thread.Sleep(_delayFunc());
            _stream.Position = value;
        }
    }
}