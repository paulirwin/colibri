using PrettyPrompt;

namespace Colibri.PromptConfig;

// Inspired from code in CSharpRepl (https://github.com/waf/CSharpRepl/blob/main/LICENSE.txt)

public static class ReplPromptConfig
{
    public static PromptConfiguration GetPromptConfig()
    {
        return new PromptConfiguration(
            prompt: ">>> "
        );
    }
    
    public static string GetPromptHistoryDirectory()
    {
        var appPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "colibri");
        Directory.CreateDirectory(appPath);
        
        return Path.Combine(appPath, "history");
    }
}