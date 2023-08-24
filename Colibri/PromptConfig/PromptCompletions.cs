using Antlr4.Runtime;
using Colibri.Core;
using PrettyPrompt.Completion;

namespace Colibri.PromptConfig;

public static class PromptCompletions
{
    public static IReadOnlyList<CompletionItem> GetCompletionItems(
        ColibriRuntime runtime, 
        string text, 
        int caret)
    {
        // HACK.PI: this code is _highly_ inefficient
        var keys = runtime.UserScope.FlattenAllKeys();
        var lexer = new ColibriLexer(new AntlrInputStream(text));
        
        lexer.RemoveErrorListeners();

        var tokens = lexer.GetAllTokens();
        
        if (tokens.Count > 0)
        {
            var lastToken = tokens[^1];
            var lastTokenText = lastToken.Text;

            if (lastToken.Type == ColibriLexer.IDENTIFIER)
            {
                keys = keys.Where(i => i.StartsWith(lastTokenText));
            }
        }

        return keys.Select(i => new CompletionItem(i)).ToList();
    }
}