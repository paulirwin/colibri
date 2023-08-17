using Colibri.Core;

namespace Colibri.Tests;

public static class TestHelper
{
    public static void DefaultTest(string input, object? expected)
    {
        var runtime = new ColibriRuntime();

        var result = runtime.EvaluateProgram(input);
        
        switch (expected)
        {
            case IEnumerable<object?> objEnumerable:
            {
                var objArr = objEnumerable as object?[] ?? objEnumerable.ToArray();

                var enumerable = result as IEnumerable<object?>;

                Assert.NotNull(enumerable);

                var list = enumerable!.ToList();

                Assert.Equal(objArr.Length, list.Count);

                for (int i = 0; i < objArr.Length; i++)
                {
                    Assert.Equal(objArr[i], list[i]);
                }

                break;
            }
            case null:
                Assert.Null(result);
                break;
            default:
                Assert.Equal(expected, result);
                break;
        }
    }
}