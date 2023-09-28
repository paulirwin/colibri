namespace Colibri.Core;

internal static class CondClauseUtility
{
    public static bool EvaluateCondClause(ColibriRuntime runtime, Scope scope, Pair clause, out object? result)
    {
        result = null;

        var test = runtime.Evaluate(scope, clause.Car);

        if (!test.IsTruthy())
            return false;

        var clauseForms = clause.ToList();

        switch (clauseForms.Count)
        {
            case 1:
                result = test;
                return true;
            case 3 when clauseForms[1] is Symbol { Value: "=>" }:
            {
                var expr = clauseForms[2];
                var proc = runtime.Evaluate(scope, expr);
                result = ColibriRuntime.TailCall(scope, new Pair(proc, new Pair(new Atom(AtomType.RuntimeReference, test), Nil.Value)));

                return true;
            }
        }

        for (int i = 1; i < clauseForms.Count; i++)
        {
            var expr = clauseForms[i];

            result = i == clauseForms.Count - 1 && expr is Pair pair and not Nil
                ? ColibriRuntime.TailCall(scope, pair) 
                : runtime.Evaluate(scope, expr);
        }

        return true;
    }

    public static object? EvaluateCondElseClause(ColibriRuntime runtime, Scope scope, Pair elseClause)
    {
        var elseExpressions = elseClause.Skip(1).ToList();

        object? result = Nil.Value;

        for (int i = 0; i < elseExpressions.Count; i++)
        {
            var expr = elseExpressions[i];

            result = i == elseExpressions.Count - 1 && expr is Pair pair and not Nil
                ? ColibriRuntime.TailCall(scope, pair) 
                : runtime.Evaluate(scope, expr);
        }

        return result;
    }
}