namespace Colibri.Tests;

public class AssociativeArrayTests
{
    [InlineData("(let [\n" +
                "    x => 4,\n" +
                "    y => 8\n" +
                "] (+ x y))", 12)]
    [InlineData("(let [ x => 4 y => 8 ] (+ x y))", 12)]
    [InlineData("(let [ x => 4, y => 8 ] (+ x y))", 12)]
    [InlineData("cond [\n" +
                "    (= 1 2) => 3\n" +
                "    (= 4 5) => 6\n" +
                "    else => 7\n" +
                "]", 7)]
    [InlineData("define items [ z => 99, y => 98, x => 97]\n" +
                ".Value (get items 0)", 99)]
    [Theory]
    public void AssociativeArray_BasicTests(string input, object? expected)
    {
        TestHelper.DefaultTest(input, expected);
    }
}