using System.Diagnostics;
using System.Text;

namespace Me;

internal static class CreateTemplate
{
    public static void Do(Dictionary<string, string> parameters) 
    {
        Debug.Assert(parameters is not null);
        Debug.Assert(parameters.Count > 0);

        var sb = new StringBuilder();

        Print.Info("ment to create template with parameters:");
        foreach (var p in parameters)
        {
            sb.AppendLine($"{p.Key} - {p.Value}");
        }
        Print.Info(sb.ToString());
    }
}
