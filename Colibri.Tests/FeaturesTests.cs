namespace Colibri.Tests;

public class FeaturesTests
{
    [InlineData("contains (features) 'r7rs", true)]
    [InlineData("contains (features) 'foo", false)]
    [InlineData("contains (features) 'colibri", true)]
    [Theory]
    public void ContainsFeatures(string input, bool expected)
    {
        TestHelper.DefaultTest(input, expected);
    }
}