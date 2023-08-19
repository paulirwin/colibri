namespace Colibri.Core;

public class RuntimeOptions
{
    public int MaxStackDepth { get; set; } = int.MaxValue;
    
    public bool ImportStandardLibrary { get; set; } = true;
}