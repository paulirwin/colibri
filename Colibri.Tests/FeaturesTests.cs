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

    // R7RS-small 4.2.1
    [InlineData("cond-expand (r7rs 42)", 42)]
    [InlineData("cond-expand (foo 42)", true)] // This is an unexpected and spec-unspecified result in Chibi, using for compatibility
    [InlineData("cond-expand (colibri 42) (else 0)", 42)]
    [InlineData("cond-expand ((and colibri r7rs) 42) (else 0)", 42)]
    [InlineData("cond-expand ((or colibri foo) 42) (else 0)", 42)]
    [InlineData("cond-expand ((not foo) 42) (else 0)", 42)]
    [InlineData("cond-expand ((library (scheme base)) 42) (else 0)", 42)]
    [InlineData("cond-expand ((library (foo bar)) 42) (else 0)", 0)]
    [InlineData("cond-expand ((and (library (scheme base)) r7rs) 42) (else 0)", 42)]
    [InlineData("cond-expand (foo 42) (r7rs 43) (else 0)", 43)]
    [Theory]
    public void CondExpandTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }
}