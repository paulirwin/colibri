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
}