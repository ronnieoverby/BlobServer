<Query Kind="Statements">
  <Reference Relative="BlobServer.Client\bin\Debug\BlobServer.Client.dll">D:\Code\BlobServer\src\BlobServer.Client\bin\Debug\BlobServer.Client.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.Http.dll</Reference>
  <NuGetReference>CoreTechs.Common</NuGetReference>
  <NuGetReference>Microsoft.AspNet.WebApi.Client</NuGetReference>
  <Namespace>BlobServer.Client</Namespace>
  <Namespace>CoreTechs.Common</Namespace>
  <Namespace>System.Net.Http</Namespace>
</Query>


using(var client = new BlobServerClient("http://localhost:63238"))
{
	var files = new DirectoryInfo(@"C:\Users\roverby\Desktop").GetFiles("*",SearchOption.AllDirectories);
	while(true)
	{
		var file = files.RandomElement();		
		var path = await client.StoreFromStreamAsync(file.OpenRead(), file.Name, rootFolder: "Batch/5");
		var bytes = await client.GetBytesAsync(path.Dump());
		await client.DeleteAsync(path);
	}
}