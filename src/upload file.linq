<Query Kind="Statements">
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.Http.dll</Reference>
  <NuGetReference>Microsoft.AspNet.WebApi.Client</NuGetReference>
  <Namespace>System.Net.Http</Namespace>
</Query>

using(HttpClient client = new HttpClient())
{
	client.BaseAddress = new Uri("http://localhost:63238/api/files");
	var resp = await client.PostAsync("?extension=.zip", new StreamContent(File.OpenRead(@"C:\Users\Ronnie\Desktop\SKYRIM OST.zip")));
	resp.Content.ReadAsStringAsync().Dump();
}


