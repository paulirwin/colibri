using Colibri.Core;

namespace Colibri.Tests;

public class BooleanTests
{
    [InlineData("(not false)", true)]
    [InlineData("(not true)", false)]
    [InlineData("(not (> 3 2))", false)]
    [InlineData("(not (< 3 2))", true)]
    [Theory]
    public void NotTests(string input, bool expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    // R7RS 6.1
    [InlineData("(eqv? #t #t)", true)]
    [InlineData("(eqv? 'a 'a)", true)]
    [InlineData(@"(eqv? #\a #\a)", true)]
    [InlineData("(eqv? '() '())", true)]
    [InlineData("(eqv? #t #f)", false)]
    [InlineData("(eqv? 'a 'b)", false)]
    [InlineData("(eqv? 42 43)", false)]
    [InlineData(@"(eqv? #\a #\b)", false)]
    [InlineData("(eqv? '() '(1 2 3))", false)]
    [InlineData("(eqv? 2 2)", true)]
    [InlineData("(eqv? 2 2.0)", false)]
    [InlineData("(eqv? 100000000 100000000)", true)]
    [InlineData("(eqv? 0 +nan.0)", false)]
    [InlineData("(eqv? (cons 1 2) (cons 1 2))", false)]
    [InlineData("(eqv? (lambda () 1) (lambda () 2))", false)]
    [InlineData("(let ((p (lambda (x) x))) (eqv? p p))", true)]
    [InlineData("(eqv? #f 'nil)", false)]
    [Theory]
    public void EqvTests(string input, bool expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    // R7RS 6.1
    [InlineData("(eq? 'a 'a)", true)]
    [InlineData("(eq? (list 'a) (list 'a))", false)]
    [InlineData("(eq? '() '())", true)]
    [InlineData("(eq? car car)", true)]
    [InlineData("(let ((x '(a))) (eq? x x))", true)]
    [InlineData("(let ((x '#())) (eq? x x))", true)]
    [InlineData("(let ((p (lambda (x) x))) (eq? p p))", true)]
    [Theory]
    public void EqTests(string input, bool expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    // R7RS 6.1
    [InlineData("(equal? 'a 'a)", true)]
    [InlineData("(equal? '(a) '(a))", true)]
    [InlineData("(equal? '(a (b) c) '(a (b) c))", true)]
    [InlineData("(equal? \"abc\" \"abc\")", true)]
    [InlineData("(equal? 2 2)", true)]
    [InlineData("(equal? (make-vector 5 'a) (make-vector 5 'a))", true)]
    // [InlineData("(equal? '#1=(a b . #1#) '#2=(a b a b . #2#))", true)] // TODO: datum labels and circular lists
    [Theory]
    public void EqualTests(string input, bool expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    // R7RS 4.2.1
    [InlineData("(and)", true)]
    [InlineData("(and (= 2 2) (> 2 1))", true)]
    [InlineData("(and (= 2 2) (< 2 1))", false)]
    [InlineData("(and 1 2 'c 42)", 42)]
    [Theory]
    public void AndTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }
    
    // R7RS 4.2.1
    [InlineData("(or)", false)]
    [InlineData("(or (= 2 2) (> 2 1))", true)]
    [InlineData("(or (= 2 2) (< 2 1))", true)]
    [InlineData("(or #f #f #f)", false)]
    [InlineData("(or 1 2 'c 42)", 1)]
    [Theory]
    public void OrTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    [Fact]
    public void AndTailCallTest()
    {
        const string program = @"
fn factorial (x) { 
    fn fact-tail (x accum) {
        if (eqv? x 0) accum (fact-tail (- x 1) (* x accum))
    }
    fact-tail x 1
}

define result (and (factorial 5) (factorial 10))
define x 1 // ensure that x parameter above is not already defined in scope or this will fail
result
";

        // a stack depth of 9 overflows for a factorial of 10 without tail calls.
        var runtime = new ColibriRuntime(new RuntimeOptions { MaxStackDepth = 9 });
        
        var result = runtime.EvaluateProgram(program);

        Assert.Equal(3628800, result);
    }
    
    [Fact]
    public void OrTailCallTest()
    {
        const string program = @"
fn factorial (x) { 
    fn fact-tail (x accum) {
        if (eqv? x 0) accum (fact-tail (- x 1) (* x accum))
    }
    fact-tail x 1
}

define result (or #f (factorial 10))
define x 1 // ensure that x is not already defined in scope or this will fail
result
";

        // a stack depth of 9 overflows for a factorial of 10 without tail calls.
        var runtime = new ColibriRuntime(new RuntimeOptions { MaxStackDepth = 9 });
        
        var result = runtime.EvaluateProgram(program);

        Assert.Equal(3628800, result);
    }

    // R7RS 6.3
    [InlineData("(boolean=?)", false)]
    [InlineData("(boolean=? #t)", true)]
    [InlineData("(boolean=? #f)", true)]
    [InlineData("(boolean=? #t #t)", true)]
    [InlineData("(boolean=? #t #f)", false)]
    [InlineData("(boolean=? #f #t)", false)]
    [InlineData("(boolean=? #f #f)", true)]
    [InlineData("(boolean=? #t #t #t)", true)]
    [InlineData("(boolean=? #t #t #f)", false)]
    [InlineData("(boolean=? #f #t #t)", false)]
    [InlineData("(boolean=? #f #f #f)", true)]
    [InlineData("(boolean=? #t #t #t #t)", true)]
    [InlineData("(boolean=? #t #t #t #f)", false)]
    [InlineData("(boolean=? #f #f #f #f)", true)]
    [InlineData("(boolean=? #t 123)", false)]
    [Theory]
    public void BooleansAreEqualTests(string input, bool expected)
    {
        TestHelper.DefaultTest(input, expected);
    }
}