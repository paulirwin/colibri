namespace Colibri.Tests;

public class ListComprehensionTests
{
    [InlineData("[from x in (range 0 5) select x]", new object[] { 0, 1, 2, 3, 4 })]
    [InlineData("[from x in (range 0 10) where (= (% x 2) 0) select x]", new object[] { 0, 2, 4, 6, 8 })]
    [InlineData("get [[from x in (range 0 3) select x]] 0", new object[] { 0, 1, 2 })]
    [Theory]
    public void ListComprehension_BasicTests(string input, object? expected)
    {
        TestHelper.DefaultTest(input, expected);
    }
}