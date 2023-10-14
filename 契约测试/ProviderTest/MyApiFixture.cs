using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Pact.NET;

namespace ProviderTest;

public class MyApiFixture : IDisposable
{
    private readonly IHost _server;
    public Uri ServerUri { get; }
    public MyApiFixture()
    {
        ServerUri = new Uri("http://localhost:9223");
        _server = Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseUrls(ServerUri.ToString());
                webBuilder.UseStartup<Startup>();
            })
            .Build();
        _server.Start();
    }

    public void Dispose()
    {
        _server.Dispose();
    }
}