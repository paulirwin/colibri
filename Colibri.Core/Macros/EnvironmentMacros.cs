namespace Colibri.Core.Macros;

public static class EnvironmentMacros
{
    public static object InteractionEnvironment(ColibriRuntime runtime, Scope scope, object?[] args) 
        => runtime.UserScope;

    public static object Environment(ColibriRuntime runtime, Scope scope, object?[] args)
    {
        var newScope = new Scope(scope.MaxStackDepth);

        foreach (var arg in args)
        {
            if (runtime.Evaluate(scope, arg) is not Pair argPair)
            {
                throw new ArgumentException("environment requires a list of import sets to import");
            }

            var importSet = ImportSet.RecursivelyResolveImportSet(scope, argPair);
            
            runtime.ImportLibrary(newScope, importSet);
        }
        
        newScope.Freeze();

        return newScope;
    }
}