namespace Colibri.Core;

public delegate object? Expression(object?[] args);

public delegate object? MacroExpression(ColibriRuntime runtime, Scope scope, object?[] args);