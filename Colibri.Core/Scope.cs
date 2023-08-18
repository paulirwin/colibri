using System.Diagnostics;
using System.Reflection.Emit;

namespace Colibri.Core;

public class Scope
{
    public Scope(int maxStackDepth)
    {
        InteropNamespaces = new HashSet<string>(Interop.DefaultNamespaces);
        MaxStackDepth = maxStackDepth;
        StackDepth = 1;
    }

    public Scope(Scope parent)
    {
        Parent = parent;
        InteropNamespaces = new HashSet<string>();
        MaxStackDepth = parent.MaxStackDepth;
        StackDepth = parent.StackDepth + 1;
    }
    
    public Scope(Scope parent, 
        int stackDepth, 
        ISet<string> interopNamespaces, 
        IDictionary<string, object?> env)
    {
        Parent = parent;
        InteropNamespaces = new HashSet<string>();
        MaxStackDepth = parent.MaxStackDepth;
        StackDepth = stackDepth;
        InteropNamespaces = interopNamespaces;
        Env = env;
    }
    
    public int MaxStackDepth { get; }
    
    public int StackDepth { get; }

    public Scope? Parent { get; }
    
    public ISet<string> InteropNamespaces { get; }

    public Procedure? ExceptionHandler { get; set; }

    public AssemblyBuilder? AssemblyBuilder { get; set; }

    public IDictionary<Symbol, RecordTypeDefinition> RecordTypes { get; } = new Dictionary<Symbol, RecordTypeDefinition>();

    public IDictionary<string, object?> Env { get; } = new Dictionary<string, object?>();

    // ReSharper disable once UnusedMember.Global - Preserved for public API
    public object? this[string key] => Resolve(key);

    public IEnumerable<string> AllInteropNamespaces()
    {
        var scope = this;

        while (scope != null)
        {
            foreach (var ns in scope.InteropNamespaces)
            {
                yield return ns;
            }

            scope = scope.Parent;
        }
    }

    public object? Resolve(string key)
    {
        var scope = this;

        while (scope != null)
        {
            if (scope.Env.TryGetValue(key, out var value))
                return value;

            scope = scope.Parent;
        }

        return null;
    }

    public bool TryResolve(string key, out object? value)
    {
        var scope = this;

        while (scope != null)
        {
            if (scope.Env.TryGetValue(key, out value))
                return true;

            scope = scope.Parent;
        }

        value = null;
        return false;
    }

    public void AddAllFrom<TValue>(IReadOnlyDictionary<string, TValue> dict)
    {
        foreach (var (key, value) in dict)
        {
            Env[key] = value;
        }
    }

    public void Define(string key, object? value)
    {
        if (Env.ContainsKey(key))
        {
            throw new ArgumentException($"Variable {key} has already been defined");
        }

        Env[key] = value;
    }

    public void DefineOrSet(string key, object? value)
    {
        Env[key] = value;
    }

    public void DefineRecordType(RecordTypeDefinition recordType)
    {
        RecordTypes[recordType.Name] = recordType;
    }

    public void Set(string key, object? value)
    {
        var scope = this;

        while (true)
        {
            if (scope.Env.ContainsKey(key))
            {
                if (scope.Parent == null)
                {
                    throw new InvalidOperationException("Global scope variables are immutable");
                }

                scope.Env[key] = value;
                break;
            }

            scope = scope.Parent ?? throw new ArgumentException($"{key} has not been defined");
        }
    }

    public Scope CreateChildScope()
    {
        if (StackDepth >= MaxStackDepth)
        {
            throw new StackOverflowException("Maximum stack depth exceeded");
        }
        
        return new Scope(this);
    }

    public Scope PopMergeScope()
    {
        Scope parent;
        int stackDepth;
        
        if (Parent == null)
        {
            parent = this;
            stackDepth = StackDepth;
        }
        else
        {
            parent = Parent;
            stackDepth = StackDepth - 1;
        }
        
        return new Scope(parent, stackDepth, InteropNamespaces, Env);
    }
}