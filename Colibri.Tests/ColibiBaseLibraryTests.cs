namespace Colibri.Tests;

public class ColibiBaseLibraryTests
{
    [InlineData("contains '(1 2 3) 2", true)]
    [InlineData("contains '(1 2 3) 4", false)]
    [Theory]
    public void ContainsTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }
}