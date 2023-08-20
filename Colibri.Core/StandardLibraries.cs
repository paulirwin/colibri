using Colibri.Core.Expressions;
using Colibri.Core.Macros;

namespace Colibri.Core;

public static class StandardLibraries
{
    public static readonly IReadOnlyList<(LibraryName Name, Library Library)> Libraries;

    static StandardLibraries()
    {
        Libraries = new List<(LibraryName Name, Library Library)>
        {
            (new LibraryName("scheme", "base"), Base),
            (new LibraryName("scheme", "case-lambda"), CaseLambda),
            (new LibraryName("scheme", "char"), Char),
            (new LibraryName("scheme", "complex"), Complex),
            (new LibraryName("scheme", "cxr"), Cxr),
            (new LibraryName("scheme", "eval"), Eval),
            (new LibraryName("scheme", "file"), File),
            (new LibraryName("scheme", "inexact"), Inexact),
            (new LibraryName("scheme", "lazy"), Lazy),
            // TODO: (scheme load) library
            (new LibraryName("scheme", "process-context"), ProcessContext),
            (new LibraryName("scheme", "read"), Read),
            // TODO: (scheme repl) library
            (new LibraryName("scheme", "time"), Time),
            (new LibraryName("scheme", "write"), Write),
            // TODO: (scheme r5rs) library
        };
    }

    public static readonly Library Base = new(new Dictionary<string, object?>
    {
        ["*"] = MathExpressions.Multiply,
        ["+"] = MathExpressions.Add,
        ["-"] = MathExpressions.Subtract,
        // ["..."] = ??? - TODO
        ["/"] = MathExpressions.Divide,
        ["<"] = BooleanExpressions.LessThan,
        ["<="] = BooleanExpressions.LessThanOrEqual,
        ["="] = BooleanExpressions.NumericallyEqual,
        // ["=>"] = ??? - TODO
        [">"] = BooleanExpressions.GreaterThan,
        [">="] = BooleanExpressions.GreaterThanOrEqual,
        ["abs"] = MathExpressions.Abs,
        ["and"] = (MacroExpression)BooleanMacros.And,
        ["append"] = ListExpressions.Append,
        ["apply"] = (MacroExpression)CoreMacros.Apply,
        // assoc, assq, assv in base.lisp
        ["begin"] = (MacroExpression)CoreMacros.Begin,
        ["binary-port?"] = PortExpressions.IsBinaryPort,
        // ["boolean=?"] = ??? - TODO
        ["boolean?"] = TypeExpressions.IsBoolean,
        ["bytevector?"] = TypeExpressions.IsBytevector,
        ["bytevector"] = BytevectorExpressions.Bytevector,
        ["bytevector-append"] = BytevectorExpressions.BytevectorAppend,
        ["bytevector-copy"] = BytevectorExpressions.BytevectorCopy,
        ["bytevector-copy!"] = BytevectorExpressions.BytevectorCopyTo,
        ["bytevector-length"] = BytevectorExpressions.BytevectorLength,
        ["bytevector-u8-ref"] = BytevectorExpressions.BytevectorU8Ref,
        ["bytevector-u8-set!"] = BytevectorExpressions.BytevectorU8Set,
        // caar and cadr in base.lisp
        ["call-with-current-continuation"] = (MacroExpression)ContinuationMacros.CallWithCurrentContinuation,
        ["call-with-port"] = (MacroExpression)PortMacros.CallWithPort,
        // ["call-with-values"] = ??? - TODO
        ["call/cc"] = (MacroExpression)ContinuationMacros.CallWithCurrentContinuation,
        ["car"] = ListExpressions.Car,
        ["cdr"] = ListExpressions.Cdr,
        // cdar and cddr in base.lisp
        ["case"] = (MacroExpression)CoreMacros.Case,
        ["ceiling"] = MathExpressions.Ceiling,
        ["char->integer"] = TypeExpressions.CharacterToInteger,
        ["char-ready?"] = (MacroExpression)PortMacros.CharReady,
        ["char=?"] = CharacterExpressions.AreEqual,
        ["char<?"] = CharacterExpressions.LessThan,
        ["char>?"] = CharacterExpressions.GreaterThan,
        ["char<=?"] = CharacterExpressions.LessThanOrEqualTo,
        ["char>=?"] = CharacterExpressions.GreaterThanOrEqualTo,
        ["char?"] = TypeExpressions.IsChar,
        ["close-input-port"] = PortExpressions.CloseInputPort,
        ["close-output-port"] = PortExpressions.CloseOutputPort,
        ["close-port"] = PortExpressions.ClosePort,
        ["complex?"] = TypeExpressions.IsComplex,
        ["cond"] = (MacroExpression)CoreMacros.Cond,
        // ["cond-expand"] = ??? - TODO
        ["cons"] = ListExpressions.Cons,
        // TODO: current-error-port, current-input-port, current-output-port
        ["define"] = (MacroExpression)CoreMacros.Define,
        ["define-record-type"] = (MacroExpression)CoreMacros.DefineRecordType,
        ["define-syntax"] = (MacroExpression)SchemeMacroMacros.DefineSyntax,
        ["define-values"] = (MacroExpression)CoreMacros.DefineValues,
        ["denominator"] = RationalExpressions.Denominator,
        ["do"] = (MacroExpression)CoreMacros.Do,
        // TODO: dynamic-wind
        // TODO: else as auxiliary syntax
        ["eof-object"] = PortExpressions.GetEofObject,
        ["eof-object?"] = PortExpressions.IsEofObject,
        ["eq?"] = BooleanExpressions.ReferencesEqual,
        ["equal?"] = BooleanExpressions.Equal,
        ["eqv?"] = BooleanExpressions.Equivalent,
        ["error"] = ExceptionExpressions.Error,
        ["error-object?"] = ExceptionExpressions.ErrorObject,
        ["error-object-irritants"] = ExceptionExpressions.ErrorObjectIrritants,
        ["error-object-message"] = ExceptionExpressions.ErrorObjectMessage,
        // even? is in base.lisp
        ["exact"] = TypeExpressions.ConvertToExact,
        ["exact?"] = TypeExpressions.IsExact,
        ["exact-integer?"] = TypeExpressions.IsExactInteger,
        ["exact-integer-sqrt"] = MathExpressions.ExactIntegerSqrt,
        ["expt"] = MathExpressions.Power,
        // TODO: features
        ["file-error?"] = ExceptionExpressions.FileError,
        ["floor"] = MathExpressions.Floor,
        ["floor/"] = MathExpressions.FloorDivide,
        ["floor-quotient"] = MathExpressions.FloorQuotient,
        ["floor-remainder"] = MathExpressions.FloorRemainder,
        ["flush-output-port"] = PortExpressions.FlushOutputPort,
        ["for-each"] = (MacroExpression)CoreMacros.ForEach,
        ["gcd"] = MathExpressions.Gcd,
        ["get-output-bytevector"] = PortExpressions.GetOutputBytevector,
        ["get-output-string"] = PortExpressions.GetOutputString,
        ["guard"] = (MacroExpression)ExceptionMacros.Guard,
        ["if"] = (MacroExpression)CoreMacros.If,
        ["include"] = (MacroExpression)CoreMacros.Include,
        //["include-ci"] = CoreMacros.Include, // TODO
        ["inexact"] = TypeExpressions.ConvertToInexact,
        ["inexact?"] = TypeExpressions.IsInexact,
        ["input-port?"] = PortExpressions.IsInputPort,
        ["input-port-open?"] = PortExpressions.IsInputPortOpen,
        ["integer?"] = TypeExpressions.IsInteger,
        ["integer->char"] = TypeExpressions.IntegerToCharacter,
        ["lambda"] = (MacroExpression)CoreMacros.Lambda,
        ["lcm"] = MathExpressions.Lcm,
        ["length"] = DynamicExpressions.Count,
        ["let"] = (MacroExpression)CoreMacros.Let,
        ["let*"] = (MacroExpression)CoreMacros.LetStar,
        ["let*-values"] = (MacroExpression)CoreMacros.LetStarValues,
        ["let-syntax"] = (MacroExpression)SchemeMacroMacros.LetSyntax,
        ["let-values"] = (MacroExpression)CoreMacros.LetValues,
        ["letrec"] = (MacroExpression)CoreMacros.Let,
        ["letrec*"] = (MacroExpression)CoreMacros.LetStar,
        ["letrec-syntax"] = (MacroExpression)SchemeMacroMacros.LetSyntax,
        ["list"] = ListExpressions.List,
        ["list->string"] = TypeExpressions.ListToString,
        // TODO: list->vector, list-copy
        // list-ref in base.lisp
        ["list-set!"] = ListExpressions.ListSet,
        // list-tail in base.lisp
        ["list?"] = TypeExpressions.IsList,
        ["make-bytevector"] = BytevectorExpressions.MakeBytevector,
        ["make-list"] = ListExpressions.MakeList,
        ["make-parameter"] = (MacroExpression)ParameterMacros.MakeParameter,
        ["make-string"] = StringExpressions.MakeString,
        ["make-vector"] = VectorExpressions.MakeVector,
        ["map"] = (MacroExpression)CoreMacros.Map,
        ["max"] = MathExpressions.Max,
        // member, memq, memv in base.lisp
        ["min"] = MathExpressions.Min,
        ["modulo"] = MathExpressions.FloorRemainder,
        // negative? in base.lisp
        ["newline"] = (MacroExpression)PortMacros.Newline,
        ["not"] = BooleanExpressions.Not,
        ["null?"] = TypeExpressions.IsNull,
        ["number?"] = TypeExpressions.IsNumber,
        ["number->string"] = TypeExpressions.NumberToString,
        ["numerator"] = RationalExpressions.Numerator,
        // odd? in base.lisp
        ["open-input-bytevector"] = PortExpressions.OpenInputBytevector,
        ["open-input-string"] = PortExpressions.OpenInputString,
        ["open-output-bytevector"] = PortExpressions.OpenOutputBytevector,
        ["open-output-string"] = PortExpressions.OpenOutputString,
        ["or"] = (MacroExpression)BooleanMacros.Or,
        ["output-port-open?"] = PortExpressions.IsOutputPortOpen,
        ["output-port?"] = PortExpressions.IsOutputPort,
        ["pair?"] = TypeExpressions.IsPair,
        ["parameterize"] = (MacroExpression)ParameterMacros.Parameterize,
        ["peek-char"] = (MacroExpression)PortMacros.PeekChar,
        ["peek-u8"] = (MacroExpression)PortMacros.PeekU8,
        ["port?"] = TypeExpressions.IsPort,
        // positive? in base.lisp
        ["procedure?"] = TypeExpressions.IsProcedure,
        ["quasiquote"] = (MacroExpression)CoreMacros.Quasiquote,
        ["quote"] = (MacroExpression)CoreMacros.Quote,
        ["quotient"] = MathExpressions.TruncateQuotient,
        ["raise"] = ExceptionExpressions.Raise,
        ["raise-continuable"] = (MacroExpression)ExceptionMacros.RaiseContinuable,
        ["rational?"] = TypeExpressions.IsRational,
        ["rationalize"] = RationalExpressions.Rationalize,
        ["read-bytevector"] = (MacroExpression)PortMacros.ReadBytevector,
        ["read-bytevector!"] = (MacroExpression)PortMacros.ReadBytevectorMutate,
        ["read-char"] = (MacroExpression)PortMacros.ReadChar,
        ["read-error?"] = ExceptionExpressions.ReadError,
        ["read-line"] = (MacroExpression)PortMacros.ReadLine,
        ["read-string"] = (MacroExpression)PortMacros.ReadString,
        ["read-u8"] = (MacroExpression)PortMacros.ReadU8,
        ["real?"] = TypeExpressions.IsReal,
        ["remainder"] = MathExpressions.TruncateRemainder,
        ["reverse"] = ListExpressions.Reverse,
        ["round"] = MathExpressions.Round,
        ["set!"] = (MacroExpression)CoreMacros.Set,
        ["set-car!"] = ListExpressions.SetCar,
        ["set-cdr!"] = ListExpressions.SetCdr,
        // square in base.lisp
        ["string"] = StringExpressions.String,
        ["string->list"] = TypeExpressions.StringToList,
        ["string->number"] = TypeExpressions.StringToNumber,
        ["string->symbol"] = TypeExpressions.StringToSymbol,
        ["string->utf8"] = TypeExpressions.StringToUtf8,
        // TODO: string->vector
        ["string-append"] = StringExpressions.StringAppend,
        ["string-copy"] = StringExpressions.StringCopy,
        ["string-copy!"] = StringExpressions.StringCopyTo,
        ["string-fill!"] = StringExpressions.StringFill,
        ["string-for-each"] = (MacroExpression)CoreMacros.StringForEach,
        ["string-length"] = StringExpressions.StringLength,
        ["string-map"] = (MacroExpression)CoreMacros.StringMap,
        ["string-ref"] = StringExpressions.StringRef,
        ["string-set!"] = StringExpressions.StringSet,
        ["string<?"] = StringExpressions.LessThan,
        ["string<=?"] = StringExpressions.LessThanOrEqualTo,
        ["string=?"] = StringExpressions.AreEqual,
        ["string>=?"] = StringExpressions.GreaterThanOrEqualTo,
        ["string>?"] = StringExpressions.GreaterThan,
        ["string?"] = TypeExpressions.IsString,
        ["substring"] = StringExpressions.Substring,
        ["symbol->string"] = TypeExpressions.SymbolToString,
        ["symbol=?"] = BooleanExpressions.SymbolEquals,
        ["symbol?"] = TypeExpressions.IsSymbol,
        // TODO: syntax-error as auxiliary syntax
        ["syntax-rules"] = (MacroExpression)SchemeMacroMacros.SyntaxRules,
        ["textual-port?"] = PortExpressions.IsTextualPort,
        ["truncate"] = MathExpressions.Truncate,
        ["truncate-quotient"] = MathExpressions.TruncateQuotient,
        ["truncate-remainder"] = MathExpressions.TruncateRemainder,
        ["truncate/"] = MathExpressions.TruncateDivide,
        ["u8-ready?"] = (MacroExpression)PortMacros.U8Ready,
        ["unless"] = (MacroExpression)BooleanMacros.Unless,
        // unquote and unquote-splicing are special forms interpreted by the runtime
        ["utf8->string"] = TypeExpressions.Utf8ToString,
        ["values"] = ValuesExpressions.Values,
        ["vector"] = VectorExpressions.Vector,
        // TODO: vector->list, vector->string
        ["vector-append"] = VectorExpressions.Append,
        ["vector-copy"] = VectorExpressions.VectorCopy,
        ["vector-copy!"] = VectorExpressions.VectorCopyTo,
        ["vector-fill!"] = VectorExpressions.VectorFill,
        ["vector-for-each"] = (MacroExpression)CoreMacros.VectorForEach,
        ["vector-length"] = VectorExpressions.VectorLength,
        ["vector-map"] = (MacroExpression)CoreMacros.VectorMap,
        ["vector-ref"] = VectorExpressions.VectorRef,
        ["vector-set!"] = VectorExpressions.VectorSet,
        ["vector?"] = TypeExpressions.IsVector,
        ["when"] = (MacroExpression)BooleanMacros.When,
        ["with-exception-handler"] = (MacroExpression)ExceptionMacros.WithExceptionHandler,
        ["write-bytevector"] = (MacroExpression)PortMacros.WriteBytevector,
        ["write-char"] = (MacroExpression)PortMacros.WriteChar,
        ["write-string"] = (MacroExpression)PortMacros.WriteString,
        ["write-u8"] = (MacroExpression)PortMacros.WriteU8,
        // zero? is in base.lisp
    }, additionalExports: new List<string>
    {
        "assoc",
        "assq",
        "assv",
        "caar",
        "cadr",
        "cdar",
        "cddr",
        "even?",
        "list-ref",
        "list-tail",
        "member",
        "memq",
        "memv",
        "negative?",
        "odd?",
        "positive?",
        "square",
        "zero?",
    })
    {
        EmbeddedResourceName = "Colibri.Core.Library.base.lisp",
    };

    public static readonly Library CaseLambda = new(new Dictionary<string, object?>
    {
        ["case-lambda"] = (MacroExpression)CoreMacros.CaseLambda,
    });

    public static readonly Library Char = new(new Dictionary<string, object?>
    {
        ["char-alphabetic?"] = CharacterExpressions.IsAlphabetic,
        ["char-ci<?"] = CharacterExpressions.CaseInsensitiveLessThan,
        ["char-ci<=?"] = CharacterExpressions.CaseInsensitiveLessThanOrEqualTo,
        ["char-ci=?"] = CharacterExpressions.CaseInsensitiveEquals,
        ["char-ci>?"] = CharacterExpressions.CaseInsensitiveGreaterThan,
        ["char-ci>=?"] = CharacterExpressions.CaseInsensitiveGreaterThanOrEqualTo,
        ["char-downcase"] = CharacterExpressions.Downcase,
        ["char-foldcase"] = CharacterExpressions.Foldcase,
        ["char-lower-case?"] = CharacterExpressions.IsLowerCase,
        ["char-numeric?"] = CharacterExpressions.IsNumeric,
        ["char-upcase"] = CharacterExpressions.Upcase,
        ["char-upper-case?"] = CharacterExpressions.IsUpperCase,
        ["char-whitespace?"] = CharacterExpressions.IsWhitespace,
        ["digit-value"] = CharacterExpressions.DigitValue,
        ["string-ci=?"] = StringExpressions.CaseInsensitiveEquals,
        ["string-ci<?"] = StringExpressions.CaseInsensitiveLessThan,
        ["string-ci<=?"] = StringExpressions.CaseInsensitiveLessThanOrEqualTo,
        ["string-ci>?"] = StringExpressions.CaseInsensitiveGreaterThan,
        ["string-ci>=?"] = StringExpressions.CaseInsensitiveGreaterThanOrEqualTo,
        ["string-downcase"] = StringExpressions.Downcase,
        ["string-foldcase"] = StringExpressions.Foldcase,
        ["string-upcase"] = StringExpressions.Upcase,
    });

    public static readonly Library Complex = new(new Dictionary<string, object?>
    {
        ["angle"] = ComplexExpressions.Angle,
        ["imag-part"] = ComplexExpressions.ImaginaryPart,
        ["magnitude"] = ComplexExpressions.Magnitude,
        ["make-polar"] = ComplexExpressions.MakePolar,
        ["make-rectangular"] = ComplexExpressions.MakeRectangular,
        ["real-part"] = ComplexExpressions.RealPart,
    });

    public static readonly Library Cxr = new(new Dictionary<string, object?>
    {
        // all defined in cxr.lisp
    }, additionalExports: new List<string>
    {
        "caaar",
        "caadr",
        "cadar",
        "caddr",
        "cdaar",
        "cdadr",
        "cddar",
        "cdddr",
        "caaaar",
        "caaadr",
        "caadar",
        "caaddr",
        "cadaar",
        "cadadr",
        "caddar",
        "cadddr",
        "cdaaar",
        "cdaadr",
        "cdadar",
        "cdaddr",
        "cddaar",
        "cddadr",
        "cdddar",
        "cddddr",
    })
    {
        EmbeddedResourceName = "Colibri.Core.Library.cxr.lisp",
    };

    public static readonly Library Eval = new(new Dictionary<string, object?>
    {
        // TODO: environment
        ["eval"] = (MacroExpression)CoreMacros.Eval,
    });

    public static readonly Library File = new(new Dictionary<string, object?>
    {
        // call-with-input-file, call-with-output-file in file.lisp
        ["delete-file"] = FileExpressions.DeleteFile,
        ["file-exists?"] = FileExpressions.FileExists,
        ["open-binary-input-file"] = PortExpressions.OpenBinaryInputFile,
        ["open-binary-output-file"] = PortExpressions.OpenBinaryOutputFile,
        ["open-input-file"] = PortExpressions.OpenInputFile,
        ["open-output-file"] = PortExpressions.OpenOutputFile,
        // TODO: write-input-from-file, write-output-to-file
    }, additionalExports: new List<string>
    {
        "call-with-input-file",
        "call-with-output-file",
    })
    {
        EmbeddedResourceName = "Colibri.Core.Library.file.lisp",
    };

    public static readonly Library Inexact = new(new Dictionary<string, object?>
    {
        // acos, asin, atan, cos, exp in inexact.lisp
        ["finite?"] = TypeExpressions.IsFinite,
        ["infinite?"] = TypeExpressions.IsInfinite,
        ["log"] = MathExpressions.Log,
        ["nan?"] = TypeExpressions.IsNaN,
        // sin in inexact.lisp
        ["sqrt"] = MathExpressions.Sqrt,
        // tan in inexact.lisp
    }, additionalExports: new List<string>
    {
        "acos",
        "asin",
        "atan",
        "cos",
        "exp",
        "sin",
        "tan",
    })
    {
        EmbeddedResourceName = "Colibri.Core.Library.inexact.lisp",
    };

    public static readonly Library Lazy = new(new Dictionary<string, object?>
    {
        ["delay"] = (MacroExpression)CoreMacros.Delay,
        ["delay-force"] = (MacroExpression)CoreMacros.DelayForce,
        ["force"] = DynamicExpressions.Force,
        ["make-promise"] = DynamicExpressions.MakePromise,
        ["promise?"] = TypeExpressions.IsPromise,
    });
    
    // TODO: (scheme load) library

    public static readonly Library ProcessContext = new(new Dictionary<string, object?>
    {
        ["command-line"] = ProcessContextExpressions.CommandLine,
        ["emergency-exit"] = ProcessContextExpressions.EmergencyExit,
        // TODO: exit
        ["get-environment-variable"] = ProcessContextExpressions.GetEnvironmentVariable,
        ["get-environment-variables"] = ProcessContextExpressions.GetEnvironmentVariables,
    });

    public static readonly Library Read = new(new Dictionary<string, object?>
    {
        ["read"] = (MacroExpression)PortMacros.Read,
    });
    
    // TODO: (scheme repl) library

    public static readonly Library Time = new(new Dictionary<string, object?>
    {
        ["current-jiffy"] = TimeExpressions.CurrentJiffy,
        ["current-second"] = TimeExpressions.CurrentSecond,
        ["jiffies-per-second"] = TimeExpressions.JiffiesPerSecond,
    });

    public static readonly Library Write = new(new Dictionary<string, object?>
    {
        ["display"] = (MacroExpression)PortMacros.Display,
        ["write"] = (MacroExpression)PortMacros.Write,
        // TODO: write-shared
        ["write-simple"] = (MacroExpression)PortMacros.Write, // HACK: since datum labels not yet supported, all writes are "simple"
    });
    
    // TODO: (scheme r5rs) library
}