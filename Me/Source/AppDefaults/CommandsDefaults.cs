using System.Text;
using System.Diagnostics;

namespace Me;

internal static class CommandsDefaults
{
    public const string ARGS_INDICATOR = "--";
    public const string PARAM_INDICATOR = "-";

    public static string[] GetArgsOrParamsDescriptionDefault(Dictionary<string, string> argsWithDescription
        , string indicator) 
    {
        Debug.Assert(argsWithDescription != null);
        Debug.Assert(indicator != null);

        var rowCount = argsWithDescription.Count;
        var result = new string[rowCount];
        int index = 0;
        var sb = new StringBuilder();
        foreach (var row in argsWithDescription)
        {
            sb.Append($"{indicator}{row.Key} - {row.Value}");
            result[index] = sb.ToString();
            ++index;
            sb.Clear();
        }
        return result;
    }

    public static void RedirectToHelp(string alias) 
    {
        Debug.Assert(alias is not null);
        Debug.Assert(Librarian.Request(alias) is not null);

        Print.Warn($"Can't execute '{alias}' withou parameters\\argunent. Redirecting to help...");

        var redirectTo = new Help();
        var subCommand = new string[] { alias };
        var subcommanded = redirectTo as ISubcommanded;

        Debug.Assert(subcommanded != null);

        subcommanded.SetSubcommand(subCommand);
        redirectTo.Execute();
    }
}
