namespace Colibri.Tests;

public class StatementBlockTests
{
    [InlineData("{\n" +
                "    def x 4\n" +
                "    set! x (+ x 2)\n" +
                "    x\n" +
                "}", 6)]
    [InlineData("(begin {\n" +
                "    + 2 4\n" +
                "})", 6)]
    [Theory]
    public void StatementBlock_BasicTests(string input, object? expected)
    {
        TestHelper.DefaultTest(input, expected);
    }
}