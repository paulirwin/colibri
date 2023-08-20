using PrettyPrompt;

namespace Colibri.PromptConfig;

public class ExitAppResult : KeyPressCallbackResult
{
    public ExitAppResult() 
        : base(string.Empty, null)
    {
    }
}