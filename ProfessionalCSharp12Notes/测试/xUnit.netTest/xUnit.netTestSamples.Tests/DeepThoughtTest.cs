namespace xUnit.netTestSamples.Tests;

public class DeepThoughtTest
{
    [Fact]
    public void ResultOfTheAnswerToTheUltimateQuestionOfLifeTheUniverseAndEverything()
    {
        // arrange�����ţ�
        int expected = 42;
        var dt = new DeepThought();

        // act���ж���
        int actual =
          dt.TheAnswerToTheUltimateQuestionOfLifeTheUniverseAndEverything();

        // assert�����ԣ�
        Assert.Equal(expected, actual);
    }
}