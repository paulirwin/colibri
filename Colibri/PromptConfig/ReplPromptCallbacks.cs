using Colibri.Core;
using PrettyPrompt;
using PrettyPrompt.Completion;
using PrettyPrompt.Consoles;
using PrettyPrompt.Documents;

namespace Colibri.PromptConfig;

public class ReplPromptCallbacks : PromptCallbacks
{
    private readonly ColibriRuntime _runtime;
    
    public ReplPromptCallbacks(ColibriRuntime runtime)
    {
        _runtime = runtime;
    }
    
    protected override IEnumerable<(KeyPressPattern Pattern, KeyPressCallbackAsync Callback)> GetKeyPressCallbacks()
    {
        yield return (new KeyPressPattern(ConsoleModifiers.Control, ConsoleKey.D),
            (_, _, _) => Task.FromResult<KeyPressCallbackResult?>(new ExitAppResult()));

        yield return (new KeyPressPattern(ConsoleModifiers.Control, ConsoleKey.C),
            (_, _, _) => Task.FromResult<KeyPressCallbackResult?>(new ExitAppResult()));
    }

    protected override Task<IReadOnlyList<CompletionItem>> GetCompletionItemsAsync(string text, int caret, TextSpan spanToBeReplaced, CancellationToken cancellationToken)
    {
        var completionItems = PromptCompletions.GetCompletionItems(_runtime, text, caret);

        return Task.FromResult<IReadOnlyList<CompletionItem>>(completionItems);
    }
}