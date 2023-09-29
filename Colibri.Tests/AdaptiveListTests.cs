namespace Colibri.Tests;

public class AdaptiveListTests
{
    [InlineData("(list? [1 2 3])", true)]
    [InlineData("get [1 2 3] 0", 1)]
    [InlineData("car [1 2 3]", 1)]
    [InlineData("cdr [1 2 3]", new object[] { 2, 3 })]
    [InlineData(".Count [1 2 3]", 3)]
    [InlineData("reverse [1 2 3]", new object[] { 3, 2, 1 })]
    [Theory]
    public void AdaptiveList_BasicTests(string input, object? expected)
    {
        TestHelper.DefaultTest(input, expected);
    }
}