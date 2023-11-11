using Xunit.Abstractions;

namespace BooksLib.Tests;

public class TestBase
{
    protected readonly ITestOutputHelper Output;

    public TestBase(ITestOutputHelper tempOutput)
    {
        Output = tempOutput;
    }
}