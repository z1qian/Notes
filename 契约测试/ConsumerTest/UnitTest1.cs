using PactNet;
using System.Net;
using System.Net.Http.Json;

namespace ConsumerTest;

public class UnitTest1
{
    private readonly IPactBuilderV4 _pactBuilder;
    public UnitTest1()
    {
        var pact = Pact.V4("MyAPI consumer", "MyAPI provider",
            new PactConfig());
        _pactBuilder = pact.WithHttpInteractions();
    }
    [Fact]
    public async Task Test1()
    {
        _pactBuilder.UponReceiving("A request to calc Abs")
            .Given("Abs")
            .WithRequest(HttpMethod.Get, "/My/Abs")
            .WithQuery("i", "-2")//Match.Integer(-2) 
            .WillRespond()
            .WithStatus(HttpStatusCode.OK)
            .WithJsonBody(2);

        await _pactBuilder.VerifyAsync(async ctx =>
        {
            using HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = ctx.MockServerUri;
            var r = await httpClient.GetFromJsonAsync<int>("/My/Abs?i=-2");
            Assert.Equal(2, r);
        });
    }
}