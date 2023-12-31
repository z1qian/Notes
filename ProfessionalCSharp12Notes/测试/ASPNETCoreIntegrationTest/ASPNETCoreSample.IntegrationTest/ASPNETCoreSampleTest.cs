﻿using Microsoft.AspNetCore.Mvc.Testing;

namespace ASPNETCoreSample.IntegrationTest;

//IClassFixture：https://www.cnblogs.com/pikqu/p/12941029.html
public class ASPNETCoreSampleTest : IClassFixture<WebApplicationFactory<Program>>
{
    class WebApplication : WebApplicationFactory<Program>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            return base.CreateHost(builder);
        }
    }

    [Fact]
    public async Task ReturnHelloWorld()
    {
        // arrange
        WebApplication app = new();
        var client = app.CreateClient();

        // act
        var response = await client.GetAsync("/");

        // assert
        response.EnsureSuccessStatusCode();
        string responseString = await response.Content.ReadAsStringAsync();
        Assert.Equal("Hello World!", responseString);
    }
}
