namespace Colibri.Core;

public class ImportSet
{
    public ImportSet(Library library)
    {
        Library = library;
        Imports = library.Exports.ToDictionary(export => export, export => export);
    }
    
    private ImportSet(Library library, IReadOnlyDictionary<string, string> imports)
    {
        Library = library;
        Imports = imports;
    }
    
    public Library Library { get; }
    
    /// <summary>
    /// A dictionary of imported names to their original names.
    /// </summary>
    public IReadOnlyDictionary<string, string> Imports { get; }
    
    public ImportSet WithOnly(IEnumerable<string> imports)
    {
        var newImports = Imports
            .Where(import => imports.Contains(import.Key))
            .ToDictionary(import => import.Key, import => import.Value);
        
        return new ImportSet(Library, newImports);
    }
    
    public ImportSet WithExcept(IEnumerable<string> imports)
    {
        var newImports = Imports
            .Where(import => !imports.Contains(import.Key))
            .ToDictionary(import => import.Key, import => import.Value);
        
        return new ImportSet(Library, newImports);
    }
    
    public ImportSet WithRename(IReadOnlyDictionary<string, string> renames)
    {
        var newImports = Imports
            .Select(import =>
            {
                var (key, value) = import;
                var newKey = renames.TryGetValue(key, out var renamedKey) ? renamedKey : key;
                return (newKey, value);
            })
            .ToDictionary(import => import.Item1, import => import.Item2);
        
        return new ImportSet(Library, newImports);
    }
    
    public ImportSet WithPrefix(string prefix)
    {
        var newImports = Imports
            .ToDictionary(import => $"{prefix}{import.Key}", import => import.Value);
        
        return new ImportSet(Library, newImports);
    }
    
    public static ImportSet RecursivelyResolveImportSet(Scope scope, Pair pair)
    {
        if (pair is
            {
                Car: Symbol { Value: "only" }, 
                Cdr: Pair
                {
                    IsList: true,
                    Car: Pair onlySourcePair,
                    Cdr: Pair onlyPair
                }
            })
        {
            var onlyKeys = new HashSet<string>();
            var onlySource = RecursivelyResolveImportSet(scope, onlySourcePair);
            
            foreach (var onlyItem in onlyPair)
            {
                if (onlyItem is not Symbol onlySymbol)
                {
                    throw new ArgumentException("only items must be symbols");
                }

                onlyKeys.Add(onlySymbol.Value);
            }

            return onlySource.WithOnly(onlyKeys);
        }

        if (pair is
            {
                Car: Symbol { Value: "except" }, 
                Cdr: Pair
                {
                    IsList: true,
                    Car: Pair exceptSourcePair,
                    Cdr: Pair exceptPair
                }
            })
        {
            var exceptKeys = new HashSet<string>();
            var exceptSource = RecursivelyResolveImportSet(scope, exceptSourcePair);
            
            foreach (var exceptItem in exceptPair)
            {
                if (exceptItem is not Symbol exceptSymbol)
                {
                    throw new ArgumentException("except items must be symbols");
                }

                exceptKeys.Add(exceptSymbol.Value);
            }

            return exceptSource.WithExcept(exceptKeys);
        }

        if (pair is
            {
                Car: Symbol { Value: "prefix" }, 
                Cdr: Pair
                {
                    IsList: true,
                    Car: Pair prefixSourcePair,
                    Cdr: Pair
                    {
                        Car: Symbol { Value: string prefix }
                    }
                }
            })
        {
            var prefixSource = RecursivelyResolveImportSet(scope, prefixSourcePair);
            
            return prefixSource.WithPrefix(prefix);
        }
        
        if (pair is
            {
                Car: Symbol { Value: "rename" }, 
                Cdr: Pair
                {
                    IsList: true,
                    Car: Pair renameSourcePair,
                    Cdr: Pair renamePair
                }
            })
        {
            var renameSource = RecursivelyResolveImportSet(scope, renameSourcePair);
            var renameMap = new Dictionary<string, string>();
            
            foreach (var renameItem in renamePair)
            {
                if (renameItem is not Pair renamePairItem)
                {
                    throw new ArgumentException("rename items must be pairs");
                }

                if (renamePairItem.Car is not Symbol fromSymbol)
                {
                    throw new ArgumentException("rename from item must be a symbol");
                }

                if (renamePairItem.Cdr is not Pair { IsList: true, Car: Symbol toSymbol })
                {
                    throw new ArgumentException("rename to item must be a symbol");
                }

                renameMap[fromSymbol.Value] = toSymbol.Value;
            }

            return renameSource.WithRename(renameMap);
        }
        
        return ResolveLibraryImportSet(scope, pair);
    }

    private static ImportSet ResolveLibraryImportSet(Scope scope, Pair pair)
    {
        var libraryName = LibraryName.Parse(pair);

        if (!scope.TryResolveLibrary(libraryName, out var library))
        {
            throw new ArgumentException($"Could not resolve library {libraryName}");
        }

        return new ImportSet(library);
    }
}