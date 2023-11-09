using Xunit.Abstractions;

namespace xUnit.netTestSamples.Tests;

public class TestBase
{
    protected readonly ITestOutputHelper Output;

    public TestBase(ITestOutputHelper tempOutput)
    {
        Output = tempOutput;
    }
}