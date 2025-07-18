using System.Text.RegularExpressions;

namespace Acontplus.Persistence.Postgres.Utilities;

public static class SqlStringParam
{
    public static string Sanitize(string input)
    {
        var expression =
            new Regex(@";|=|<|>| or | and |select 
              | insert | update | drop | xp_ | --| exec"
            );

        var result =
            expression.Replace(input, MatchEvaluatorHandler);

        return result;
    }

    private static string MatchEvaluatorHandler(Match match)
    {
        //Replace the matched items with a blank string of 
        //equal length
        return new string(' ', match.Length);
    }
}
