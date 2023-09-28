using System.Globalization;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Rationals;

namespace Colibri.Core;

public class ColibriVisitor : ColibriParserBaseVisitor<Node>
{
    private static readonly Regex _complexRegex = new(@"(?<real>((-?[0-9]+(\.[0-9]*)?)|[\-+]inf\.0|[\-+]nan\.0))(?<imaginary>[\-+](([0-9]+(\.[0-9]*)?)|inf\.0|nan\.0))i", RegexOptions.Compiled);

    public override Node VisitProg(ColibriParser.ProgContext context)
    {
        var node = new Program();

        foreach (var child in context.children)
        {
            var childNode = Visit(child);

            if (childNode != null)
            {
                node.Children.Add(childNode);
            }
        }

        return node;
    }

    public override Node VisitList(ColibriParser.ListContext context)
    {
        if (context.children.Count == 2
            && context.children[0] is ITerminalNode { Symbol: CommonToken { Text: "(" } }
            && context.children[1] is ITerminalNode { Symbol: CommonToken { Text: ")" } })
        {
            return Nil.Value;
        }

        var nodes = new List<Node>();
        bool isComplete = false;

        foreach (var child in context.children)
        {
            var childNode = Visit(child);

            if (childNode != null)
            {
                if (childNode is Symbol { Value: "unquote" or "unquote-splicing" } unquoteSym)
                {
                    if (context.children.Count != 4) // ["(", "unquote" or "unquote-splicing", <some value>, ")"]
                    {
                        throw new ArgumentException("unquote or unquote-splicing procedural forms require exactly one argument");
                    }

                    var unquoteChild = context.children[2]; // <some value> above

                    var node = Visit(unquoteChild);

                    return new Unquote(node, unquoteSym.Value == "unquote-splicing");
                }

                nodes.Add(childNode);
            }
            else if (child is ITerminalNode { Symbol: CommonToken { Text: ")" } })
            {
                isComplete = true;
            }
        }

        if (!isComplete)
        {
            throw new IncompleteParseException();
        }

        if (nodes.Count >= 3 && nodes[^2] is Symbol { Value: "." })
        {
            return List.ImproperListFromNodes(nodes.Take(nodes.Count - 2).Append(nodes[^1]));
        }

        return List.FromNodes(nodes);
    }

    public override Node VisitVector(ColibriParser.VectorContext context)
    {
        var nodes = new List<Node>();
        bool isComplete = false;

        foreach (var child in context.children)
        {
            var childNode = Visit(child);

            if (childNode != null)
            {
                nodes.Add(childNode);
            }
            else if (child is ITerminalNode { Symbol: CommonToken { Text: ")" or "]" } })
            {
                isComplete = true;
            }
        }

        if (!isComplete)
        {
            throw new IncompleteParseException();
        }
        
        return new Vector(nodes);
    }

    public override Node VisitBytevector(ColibriParser.BytevectorContext context)
    {
        var bv = new Bytevector();
        bool isComplete = false;

        foreach (var child in context.children)
        {
            var childNode = Visit(child);

            switch (childNode)
            {
                case null:
                {
                    if (child is ITerminalNode { Symbol: CommonToken { Text: ")" } })
                    {
                        isComplete = true;
                    }

                    continue;
                }
                case Atom { AtomType: AtomType.Number, Value: >= 0 and <= 255 } atom:
                    bv.Add((byte)(int)atom.Value);
                    break;
                default:
                    throw new InvalidOperationException("Only integer literals between 0-255 are supported for bytevector literal values.");
            }
        }

        if (!isComplete)
        {
            throw new IncompleteParseException();
        }

        return bv;
    }

    public override Node VisitInteger(ColibriParser.IntegerContext context)
    {
        var integer = context.INTEGER();

        if (integer != null)
        {
            var i = int.Parse(integer.GetText());
            return new Atom(AtomType.Number, i);
        }

        throw new NotImplementedException("Unknown integer type");
    }

    public override Node VisitRegex(ColibriParser.RegexContext context)
    {
        var literal = context.REGEX_PATTERN();

        if (literal == null)
        {
            throw new InvalidOperationException("Regex pattern unable to be parsed");
        }

        var text = literal.GetText()[1..];
        var lastSlashPos = text.LastIndexOf('/');

        var pattern = text[..lastSlashPos];
        var flags = text[lastSlashPos..].TrimStart('/');

        return new RegexLiteral(pattern, flags);
    }

    public override Node VisitAtom(ColibriParser.AtomContext context)
    {
        var number = context.number();

        if (number != null)
        {
            return ParseNumber(number);
        }

        var str = context.STRING();

        if (str != null)
        {
            var strText = str.GetText()[1..^1]; // exclude start and end quotes
            var unescapedStr = UnescapeString(strText);
            return new Atom(AtomType.String, unescapedStr);
        }

        var symbol = context.symbol();

        if (symbol != null)
        {
            return VisitSymbol(symbol);
        }

        var character = context.CHARACTER();

        if (character != null)
        {
            return ParseCharacter(character);
        }

        throw new NotImplementedException("Unknown atom type");
    }

    private static Node ParseCharacter(IParseTree character)
    {
        var charText = character.GetText()[2..];
        char c = charText.ToLowerInvariant() switch
        {
            "" => ' ',
            "alarm" => '\u0007',
            "backspace" => '\u0008',
            "delete" => '\u007f',
            "escape" => '\u001b',
            "newline" => '\n',
            "null" => '\0',
            "return" => '\r',
            "space" => ' ',
            "tab" => '\t',
            _ => charText[0]
        };

        if (c == 'x' && charText.Length > 1)
        {
            var hex = charText[1..];

            if (!ushort.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var hexCode))
            {
                throw new ArgumentException($"Invalid hex character escape: #\\{charText}");
            }

            c = (char)hexCode;
        }

        return new Atom(AtomType.Character, c);
    }

    public override Node VisitSymbol(ColibriParser.SymbolContext symbol)
    {
        if (symbol.identifier() is { } identifier)
        {
            return VisitIdentifier(identifier);
        }

        var symbolText = symbol.GetText();

        return symbolText switch
        {
            "#t" or "#true" or "true" => new Atom(AtomType.Boolean, true),
            "#f" or "#false" or "false" => new Atom(AtomType.Boolean, false),
            _ => new Symbol(symbolText)
        };
    }

    private static Node ParseNumber(ColibriParser.NumberContext number)
    {
        var prefixedNumber = number.prefixed_number();

        if (prefixedNumber != null)
        {
            return ParsePrefixedNumber(prefixedNumber);
        }

        var floatingPoint = number.FLOAT();

        if (floatingPoint != null)
        {
            double num = Convert.ToDouble(floatingPoint.GetText().Replace("_", ""));
            return new Atom(AtomType.Number, num);
        }

        var integer = number.INTEGER();

        if (integer != null)
        {
            int num = Convert.ToInt32(integer.GetText().Replace("_", ""));
            return new Atom(AtomType.Number, num);
        }

        var complex = number.COMPLEX();

        if (complex != null)
        {
            return ParseComplex(complex.GetText().Replace("_", ""));
        }

        var ratio = number.RATIO();

        if (ratio != null)
        {
            return ParseRational(ratio.GetText().Replace("_", ""));
        }

        var posInfinity = number.POS_INFINITY();

        if (posInfinity != null)
        {
            return new Atom(AtomType.Number, double.PositiveInfinity);
        }

        var negInfinity = number.NEG_INFINITY();

        if (negInfinity != null)
        {
            return new Atom(AtomType.Number, double.NegativeInfinity);
        }

        var nan = number.NAN();

        if (nan != null)
        {
            return new Atom(AtomType.Number, double.NaN);
        }

        throw new NotImplementedException("Unknown number type");
    }

    private static Node ParsePrefixedNumber(ColibriParser.Prefixed_numberContext prefixedNumber)
    {
        var binary = prefixedNumber.binary_prefixed();

        if (binary != null)
        {
            return ParseBinaryNumber(binary);
        }

        var octal = prefixedNumber.octal_prefixed();

        if (octal != null)
        {
            return ParseOctalNumber(octal);
        }

        var decimalNum = prefixedNumber.decimal_prefixed();

        if (decimalNum != null)
        {
            return ParseDecimalNumber(decimalNum);
        }

        var hex = prefixedNumber.hex_prefixed();

        if (hex != null)
        {
            return ParseHexNumber(hex);
        }

        throw new NotImplementedException("Unknown prefixed number type");
    }

    private static Node ParseHexNumber(IParseTree hex)
    {
        var number = hex.GetText()[2..].Replace("_", ""); // trim off #x

        int value = int.Parse(number, NumberStyles.HexNumber);

        return new Atom(AtomType.Number, value);
    }

    private static bool? ParseExactPrefixDesignator(string prefixText)
    {
        if (prefixText.Contains('e'))
        {
            return true;
        }
            
        if (prefixText.Contains('i'))
        {
            return false;
        }

        return null;
    }

    private static Node ParseDecimalNumber(ColibriParser.Decimal_prefixedContext decimalNum)
    {
        var prefix = decimalNum.DECIMAL_PREFIX();
        var prefixText = prefix.GetText().TrimStart('#');
        bool? exact = ParseExactPrefixDesignator(prefixText);

        var floatingPoint = decimalNum.FLOAT();

        if (floatingPoint != null)
        {
            var floatText = floatingPoint.GetText().Replace("_", "");

            if (exact == true)
            {
                decimal num = Convert.ToDecimal(floatText);
                return new Atom(AtomType.Number, num);
            }
            else
            {
                double num = Convert.ToDouble(floatText);
                return new Atom(AtomType.Number, num);
            }
        }

        var integer = decimalNum.INTEGER();

        if (integer != null)
        {
            var intText = integer.GetText().Replace("_", "");

            if (exact == false)
            {
                double num = Convert.ToDouble(intText);
                return new Atom(AtomType.Number, num);
            }
            else
            {
                int num = Convert.ToInt32(intText);
                return new Atom(AtomType.Number, num);
            }
        }

        throw new NotImplementedException("Unknown prefixed decimal number type");
    }

    private static Node ParseOctalNumber(IParseTree octal)
    {
        var number = octal.GetText()[2..].Replace("_", ""); // trim off #o

        int value = Convert.ToInt32(number, 8);

        return new Atom(AtomType.Number, value);
    }

    private static Node ParseBinaryNumber(IParseTree binary)
    {
        var number = binary.GetText()[2..].Replace("_", ""); // trim off #b

        int value = Convert.ToInt32(number, 2);

        return new Atom(AtomType.Number, value);
    }

    private static Node ParseRational(string text)
    {
        var rational = Rational.Parse(text);

        return new Atom(AtomType.Number, rational);
    }

    private static Node ParseComplex(string text)
    {
        var match = _complexRegex.Match(text);

        if (!match.Success)
        {
            throw new InvalidOperationException($"Unable to parse complex number: {text}");
        }

        var real = match.Groups["real"].Value switch
        {
            "-inf.0" => double.NegativeInfinity,
            "+inf.0" => double.PositiveInfinity,
            "-nan.0" or "+nan.0" => double.NaN,
            { } other => double.Parse(other)
        };

        var imaginary = match.Groups["imaginary"].Value switch
        {
            "-inf.0" => double.NegativeInfinity,
            "+inf.0" => double.PositiveInfinity,
            "-nan.0" or "+nan.0" => double.NaN,
            { }
                other => double.Parse(other)
        };

        return new Atom(AtomType.Number, new Complex(real, imaginary));
    }

    public override Node VisitMeta(ColibriParser.MetaContext context)
    {
        var quote = context.quote();

        if (quote != null)
        {
            var child = quote.children[1];

            var node = Visit(child);

            return new Quote(node);
        }

        var quasiquote = context.quasiquote();

        if (quasiquote != null)
        {
            var child = quasiquote.children[1];

            var node = Visit(child);

            return new Quasiquote(node);
        }

        var unquoteSplicing = context.unquote_splicing();

        if (unquoteSplicing != null)
        {
            var child = unquoteSplicing.children[1];

            var node = Visit(child);

            return new Unquote(node, true);
        }

        var unquote = context.unquote();

        if (unquote != null)
        {
            var child = unquote.children[1];

            var node = Visit(child);

            return new Unquote(node, false);
        }

        var commentDatum = context.comment_datum();

        if (commentDatum != null)
        {
            return null!;
        }

        throw new NotImplementedException("Unknown macro type");
    }

    private static string UnescapeString(string input)
    {
        var sb = new StringBuilder();

        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];

            if (c != '\\')
            {
                sb.Append(c);
            }
            else if (i < input.Length - 1)
            {
                char n = input[i + 1];
                char? u = n switch
                {
                    'a' => '\a',
                    'b' => '\b',
                    'f' => '\f',
                    'n' => '\n',
                    'r' => '\r',
                    't' => '\t',
                    'v' => '\v',
                    '0' => '\0',
                    '\\' => '\\',
                    '\"' => '\"',
                    '|' => '|',
                    _ => null
                };

                if (u != null)
                {
                    sb.Append(u);
                    i++;
                    continue;
                }

                switch (n)
                {
                    case 'u' when input.Length - 2 - i < 4:
                        throw new InvalidOperationException("Not enough characters left in the string for a 16-bit unicode escape");
                    case 'u':
                    {
                        ushort val = ushort.Parse(input.Substring(i + 2, 4), NumberStyles.HexNumber);
                        sb.Append((char)val);
                        i += 5;
                        break;
                    }
                    case 'x':
                    {
                        int scPos = input.IndexOf(';', i + 1);

                        if (scPos < 0)
                        {
                            throw new InvalidOperationException("Unicode escape sequences starting with \\x must end in a semicolon.");
                        }

                        string seq = input.Substring(i + 2, scPos - i - 2);
                        ushort val = ushort.Parse(seq, NumberStyles.HexNumber);
                        sb.Append((char)val);
                        i += 2 + seq.Length;
                        break;
                    }
                    default:
                        throw new InvalidOperationException($"Unknown string escape sequence: \\{n}");
                }
            }
        }

        return sb.ToString();
    }

    public override Node VisitStatementBlock(ColibriParser.StatementBlockContext context)
    {
        var nodes = context.statementExpr()
            .Select(VisitStatementExpr)
            .ToList();

        return new StatementBlock(nodes);
    }

    public override Node VisitStatementExpr(ColibriParser.StatementExprContext context)
    {
        var expressions = context.expr();
        
        if (context.identifier() is { } identifier)
        {
            if (expressions.Length == 0)
            {
                return VisitIdentifier(identifier);
            }
            
            var listNodes = new List<Node>
            {
                VisitIdentifier(identifier)
            };
            
            listNodes.AddRange(context.expr().Select(VisitExpr));

            return List.FromNodes(listNodes);
        }

        if (expressions.Length is 0 or > 1)
        {
            throw new InvalidOperationException("Unable to parse statement block; too many expressions");
        }

        return VisitExpr(expressions[0]);
    }

    public override Node VisitIdentifier(ColibriParser.IdentifierContext context)
    {
        var escapedSymbol = context.ESCAPED_IDENTIFIER();

        if (escapedSymbol != null)
        {
            var escapedText = context.GetText()[1..^1]; // exclude start and end bars
            var unescapedText = UnescapeString(escapedText);
            return new Symbol(unescapedText, escaped: true);
        }

        return context.GetText() switch
        {
            "#t" or "#true" or "true" => new Atom(AtomType.Boolean, true),
            "#f" or "#false" or "false" => new Atom(AtomType.Boolean, false), 
            string s => new Symbol(s),
        };
    }

    public override Node VisitAdaptiveCollection(ColibriParser.AdaptiveCollectionContext context)
    {
        var nodes = new List<Node>();
        bool isComplete = false;

        foreach (var child in context.children)
        {
            var childNode = Visit(child);

            if (childNode != null)
            {
                nodes.Add(childNode);
            }
            else if (child is ITerminalNode { Symbol: CommonToken { Text: "]" } })
            {
                isComplete = true;
            }
        }

        if (!isComplete)
        {
            throw new IncompleteParseException();
        }

        if (!nodes.Any(i => i is Symbol { Value: "=>" }))
        {
            return new AdaptiveList(nodes);
        }

        if (nodes.Count % 3 != 0)
        {
            throw new InvalidOperationException("Incorrect number of elements for an associative array");
        }

        var assocArray = new AssociativeArray();

        Node? key = null;

        for (int i = 0; i < nodes.Count; i++)
        {
            switch (i % 3)
            {
                case 0:
                    key = nodes[i];
                    break;
                case 1 when nodes[i] is not Symbol { Value: "=>" }:
                    throw new InvalidOperationException("Missing expected => in associative array");
                case 2 when key == null:
                    throw new InvalidOperationException("Key is null");
                case 2:
                    assocArray.Add(key, nodes[i]);
                    key = null;
                    break;
            }
        }
        
        return assocArray;
    }
}