namespace xUnit.netTestSamples.Tests;

public class DeepThoughtTest
{
    [Fact]
    public void ResultOfTheAnswerToTheUltimateQuestionOfLifeTheUniverseAndEverything()
    {
        // arrange£®∞≤≈≈£©
        int expected = 42;
        var dt = new DeepThought();

        // act£®––∂Ø£©
        int actual =
          dt.TheAnswerToTheUltimateQuestionOfLifeTheUniverseAndEverything();

        // assert£®∂œ—‘£©
        Assert.Equal(expected, actual);
    }
}