using System.Diagnostics;
using System.Text;

namespace Me;

internal static class CreateProject
{
    public static void Do(Dictionary<string, string> parameters) 
    {
        Debug.Assert(parameters is not null);
        Debug.Assert(parameters.Count > 0);

        var sb = new StringBuilder();

        Print.Info("ment to create project with parameters:");
        foreach (var p in parameters) 
        {
            sb.AppendLine($"{p.Key} - {p.Value}");
        }
        Print.Info(sb.ToString());
    }
}
