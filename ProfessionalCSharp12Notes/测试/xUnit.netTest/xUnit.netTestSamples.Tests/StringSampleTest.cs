using Xunit.Abstractions;

namespace xUnit.netTestSamples.Tests;

public class StringSampleTest : TestBase
{
    public StringSampleTest(ITestOutputHelper tempOutput) : base(tempOutput)
    {
    }

    [Fact]
    public void GetStringDemoExceptions()
    {
        StringSample sample = new(string.Empty);
        StringSample sample2;
        try
        {
            sample2 = new StringSample(null!);
        }
        catch (Exception ex)
        {
            Output.WriteLine($"捕获到【{ex.GetType()}】类型的异常：{ex.Message}");
        }

        Assert.Throws<ArgumentNullException>(() => sample.GetStringDemo(null!, "a"));
        Assert.Throws<ArgumentNullException>(() => sample.GetStringDemo("a", null!));
        Assert.Throws<ArgumentException>(() => sample.GetStringDemo(string.Empty, "a"));
    }

    [Fact]
    public void GetStringDemoBNotInA()
    {
        // arrange
        string expected = "b not found in a";
        StringSample sample = new(string.Empty);

        // act
        string actual = sample.GetStringDemo("a", "b");
        Output.WriteLine(actual);

        // assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("", "a", "b", "b not found in a")]
    [InlineData("", "longer string", "nger", "removed nger from longer string: lo string")]
    [InlineData("init", "longer string", "string", "INIT")]
    public void GetStringDemoInlineData(string init, string a, string b, string expected)
    {
        StringSample sample = new(init);
        string actual = sample.GetStringDemo(a, b);
        Output.WriteLine(actual);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(GetStringSampleData))]
    public void GetStringDemoMemberData(string init, string a, string b, string expected)
    {
        StringSample sample = new(init);
        string actual = sample.GetStringDemo(a, b);
        Assert.Equal(expected, actual);
    }

    public static IEnumerable<object[]> GetStringSampleData() =>
        new[]
        {
                new object[] { "", "a", "b", "b not found in a" },
                new object[] { "init", "longer string", "string", "INIT" },
                new object[] { "", "longer string", "nger", "removed nger from longer string: lo string" }
        };
}
