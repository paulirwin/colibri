namespace Colibri.Core;

public class Procedure : IInvokable
{
    public Procedure(string text, Node parameters, Node[] body)
    {
        Text = text;
        Parameters = parameters;
        Body = body;
    }

    public string Text { get; }

    public Node Parameters { get; }

    public Node[] Body { get; }
    
    public Node? ReturnType { get; set; }

    public override string ToString() => Text;

    public object? Invoke(ColibriRuntime runtime, Scope scope, object?[] args) => Invoke(runtime, scope, args, false);

    public object? Invoke(ColibriRuntime runtime, Scope scope, object?[] args, bool disableTailCalls)
    {
        var childScope = scope.CreateChildScope();

        switch (Parameters)
        {
            case Symbol pSymbol:
                childScope.Define(pSymbol.Value, List.FromNodes(args));
                break;
            case Pair { IsList: true } pair:
            {
                var list = pair.ToList();

                for (int i = 0; i < list.Count; i++)
                {
                    if (args.Length <= i)
                    {
                        break;
                    }
                
                    var arg = args[i];
                    string identifier;
                    var argElement = list.ElementAt(i);
                
                    switch (argElement)
                    {
                        case Symbol symbol:
                            identifier = symbol.Value;
                            break;
                        case TypedIdentifier typedIdentifier:
                            identifier = typedIdentifier.Identifier.Value;

                            CheckType(runtime, childScope, typedIdentifier.Type, arg,
                                (expectedType, actualType) =>
                                    throw new ArgumentTypeCheckException(identifier, expectedType, actualType));
                            break;
                        default:
                            throw new ArgumentException($"Unhandled parameter node type: {list[i]?.GetType().ToString() ?? "null"}");
                    }

                    childScope.Define(identifier, arg);
                }

                break;
            }
            case Pair { IsList: false } improperList:
            {
                var list = improperList.ToList();

                for (int i = 0; i < list.Count; i++)
                {
                    if (list.ElementAt(i) is not Symbol symbol)
                    {
                        throw new ArgumentException($"Unhandled parameter node type: {list[i]?.GetType().ToString() ?? "null"}");
                    }

                    if (i == list.Count - 1)
                    {
                        childScope.Define(symbol.Value, List.FromNodes(args.Skip(i)));

                        break;
                    }

                    if (args.Length > i)
                    {
                        var arg = args[i];

                        childScope.Define(symbol.Value, arg);
                    }
                }

                break;
            }
        }

        object? result = Nil.Value;

        for (int i = 0; i < Body.Length; i++)
        {
            var bodyNode = Body[i];

            if (i < Body.Length - 1)
            {
                result = runtime.Evaluate(childScope, bodyNode);
            }
            else
            {
                result = bodyNode is Pair pair && !disableTailCalls ? ColibriRuntime.TailCall(childScope, pair) : runtime.Evaluate(childScope, bodyNode);
            }
        }

        if (ReturnType is not null)
        {
            CheckType(runtime, childScope, ReturnType, result, 
                (expectedType, actualType) => throw new ReturnTypeCheckException(expectedType, actualType));
        }

        return result;
    }

    private static void CheckType(ColibriRuntime runtime, 
        Scope childScope, 
        Node expectedType, 
        object? value,
        Action<Type, Type?> onTypeMismatch)
    {
        var resolvedTypeNode = expectedType is Symbol 
            ? runtime.Evaluate(childScope, expectedType)
            : expectedType;

        var resolvedType = resolvedTypeNode switch
        {
            Nil => typeof(Nil),
            Type type => type,
            Pair => typeof(Pair),
            _ => throw new InvalidOperationException($"Type symbol {expectedType} did not resolve to a type")
        };

        switch (value)
        {
            case Nil when resolvedType != typeof(Nil) && resolvedType != typeof(void):
                onTypeMismatch(resolvedType, typeof(Nil));
                break;
            case null when resolvedType.IsValueType:
                onTypeMismatch(resolvedType, null);
                break;
        }

        var actualType = value?.GetType();

        if (value is not null && actualType is not null && !actualType.IsAssignableTo(resolvedType))
        {
            onTypeMismatch(resolvedType, actualType);
        }
    }
}