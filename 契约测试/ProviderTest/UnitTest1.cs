using PactNet.Infrastructure.Outputters;
using PactNet.Output.Xunit;
using PactNet.Verifier;
using Xunit.Abstractions;

namespace ProviderTest;

public class UnitTest1 : IClassFixture<MyApiFixture>
{
    private readonly MyApiFixture _fixture;
    private readonly ITestOutputHelper _output;
    public UnitTest1(MyApiFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _output = output;
    }
    [Fact]
    public async Task Test1()
    {
        var config = new PactVerifierConfig
        {
            Outputters = new List<IOutput>
            {
                new XunitOutput(_output),
            },
        };
        string pactPath = Path.Combine("C:\\Users\\67269\\Desktop",
            "ConsumerTest", "pacts", "MyAPI consumer-MyAPI provider.json");
        using var pactVerifier = new PactVerifier("MyAPI provider", config);
        pactVerifier
            .WithHttpEndpoint(_fixture.ServerUri)
            .WithFileSource(new FileInfo(pactPath))
            .Verify();
    }
}