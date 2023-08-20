using PrettyPrompt;
using PrettyPrompt.Consoles;

namespace Colibri.PromptConfig;

public class ReplPromptCallbacks : PromptCallbacks
{
    protected override IEnumerable<(KeyPressPattern Pattern, KeyPressCallbackAsync Callback)> GetKeyPressCallbacks()
    {
        yield return (new KeyPressPattern(ConsoleModifiers.Control, ConsoleKey.D),
            (_, _, _) => Task.FromResult<KeyPressCallbackResult?>(new ExitAppResult()));

        yield return (new KeyPressPattern(ConsoleModifiers.Control, ConsoleKey.C),
            (_, _, _) => Task.FromResult<KeyPressCallbackResult?>(new ExitAppResult()));
    }
}