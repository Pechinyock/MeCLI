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
}
