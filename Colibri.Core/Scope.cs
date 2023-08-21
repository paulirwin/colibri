using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;

namespace Colibri.Core;

public class Scope
{
    private readonly Dictionary<Symbol, RecordTypeDefinition> _recordTypes = new();
    private readonly Dictionary<string, object?> _env = new();
    private readonly Dictionary<LibraryName, Library> _availableLibraries = new();
    private readonly HashSet<string> _interopNamespaces;

    public Scope(int maxStackDepth)
    {
        _interopNamespaces = new HashSet<string>(Interop.DefaultNamespaces);
        MaxStackDepth = maxStackDepth;
        StackDepth = 1;
    }

    public Scope(Scope parent)
    {
        Parent = parent;
        Mutable = parent.Mutable;
        _interopNamespaces = new HashSet<string>();
        MaxStackDepth = parent.MaxStackDepth;
        StackDepth = parent.StackDepth + 1;
    }

    private Scope(Scope parent, 
        bool mutable,
        int stackDepth, 
        HashSet<string> interopNamespaces, 
        Dictionary<LibraryName, Library> availableLibraries,
        Dictionary<string, object?> env)
    {
        Parent = parent;
        Mutable = mutable;
        _interopNamespaces = new HashSet<string>();
        MaxStackDepth = parent.MaxStackDepth;
        StackDepth = stackDepth;
        _interopNamespaces = interopNamespaces;
        _availableLibraries = availableLibraries;
        _env = env;
    }

    public bool Mutable { get; private set; } = true;
    
    public int MaxStackDepth { get; }
    
    public int StackDepth { get; }

    public Scope? Parent { get; }

    public IReadOnlyDictionary<LibraryName, Library> AvailableLibraries => _availableLibraries;

    public IReadOnlySet<string> InteropNamespaces => _interopNamespaces;

    public Procedure? ExceptionHandler { get; set; }

    public AssemblyBuilder? AssemblyBuilder { get; set; }

    public IReadOnlyDictionary<Symbol, RecordTypeDefinition> RecordTypes => _recordTypes;

    public IReadOnlyDictionary<string, object?> Env => _env;

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
            EnsureMutable(key);
            _env[key] = value;
        }
    }

    public void Define(string key, object? value)
    {
        EnsureMutable(key);
        
        if (_env.ContainsKey(key))
        {
            throw new ArgumentException($"Variable {key} has already been defined");
        }

        _env[key] = value;
    }

    public void DefineOrSet(string key, object? value)
    {
        EnsureMutable(key);
        _env[key] = value;
    }

    public void DefineRecordType(RecordTypeDefinition recordType)
    {
        EnsureMutable(recordType.Name.Value);
        _recordTypes[recordType.Name] = recordType;
    }

    public void Set(string key, object? value)
    {
        EnsureMutable(key);
        
        var scope = this;

        while (true)
        {
            if (scope.Env.ContainsKey(key))
            {
                if (scope.Parent == null)
                {
                    throw new InvalidOperationException("Global scope variables are immutable");
                }

                scope._env[key] = value;
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
        
        return new Scope(parent, Mutable, stackDepth, _interopNamespaces, _availableLibraries, _env);
    }

    public bool TryResolveLibrary(LibraryName name, [NotNullWhen(true)] out Library? library)
    {
        var scope = this;

        while (scope != null)
        {
            if (scope.AvailableLibraries.TryGetValue(name, out library))
                return true;

            scope = scope.Parent;
        }

        library = null;
        return false;
    }

    public IEnumerable<string> FlattenAllKeys()
    {
        var scope = this;

        while (scope != null)
        {
            foreach (var key in scope.Env.Keys)
            {
                yield return key;
            }

            scope = scope.Parent;
        }
    }

    public bool HasAvailableLibrary(LibraryName libraryName)
    {
        var scope = this;

        while (scope != null)
        {
            if (scope.AvailableLibraries.ContainsKey(libraryName))
            {
                return true;
            }

            scope = scope.Parent;
        }

        return false;
    }
    
    private void EnsureMutable(string key)
    {
        if (!Mutable)
        {
            throw new EnvironmentImmutableException(key);
        }
    }

    public void AddLibrary(LibraryName name, Library library)
    {
        EnsureMutable(name.ToString());
        _availableLibraries[name] = library;
    }

    public void AddInteropNamespace(string ns)
    {
        EnsureMutable(ns);
        _interopNamespaces.Add(ns);
    }

    /// <summary>
    /// Prevents the scope from being mutated. Note that this does not prevent child scopes created before this is
    /// called from being mutated. Therefore, this method should be called as early as possible in the scope's lifetime
    /// before any child scopes are created.
    /// </summary>
    public void Freeze() => Mutable = false;
}