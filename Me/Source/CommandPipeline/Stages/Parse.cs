using System.Diagnostics;
using System.Text;

namespace Me;

internal sealed class Parse : StageBase
{
    public Parse(IPipelineContext context) : base(context) {}

    public override event Action<string> OnFailure;

    public override bool Proceed()
    {
        var cmdToParse = _context.GetExecutingCommand();
        var sourceInput = _context.GetSourceInput();

        Debug.Assert(sourceInput != null && sourceInput.Length > 0);
        Debug.Assert(cmdToParse != null);

        if (cmdToParse.IsArgumented()) 
            FillArguments(sourceInput, cmdToParse);

        if(cmdToParse.IsParametrized())
            FillParameters(sourceInput, cmdToParse);

        if(cmdToParse.IsExternal())
            FillExternal(sourceInput, cmdToParse);

        return true;
    }

    private void FillArguments(string[] input, MeCommandBase foundedInRegistry)
    {
        var argumented = foundedInRegistry as IArgumented;
        Debug.Assert(argumented is not null, $"{foundedInRegistry.Alias} marked as argumented but doesn't implement {nameof(IArgumented)}");

        var indicators = argumented.GetArgumentIndicator();
        var argumetns = GetArguments(input, indicators);
        argumented.SetArguments(argumetns);
    }

    private void FillExternal(string[] input, MeCommandBase foundedInRegistry)
    {
        var external = foundedInRegistry as IExternal;
        Debug.Assert(external is not null, $"{foundedInRegistry.Alias} marked as external but doesn't implement {nameof(IExternal)}");

        if (input.Length <= 1)
            return;

        external.SetProgramName(input[1]);
        var sb = new StringBuilder();
        for (int i = 2; i < input.Length; ++i)
        {
            sb.Append($" {input[i]}");
        }

        external.SetArguments(sb.ToString());
    }

    private void FillParameters(string[] input, MeCommandBase foundedInRegistry)
    {
    }

    private string[] GetArguments(string[] tokens, string argIndicator)
    {
        if (tokens.Length <= 1)
            return null;

        var args = new List<string>(tokens.Length - 1);

        for (int i = 1; i < tokens.Length; ++i)
        {
            if (String.IsNullOrEmpty(tokens[i]))
                continue;

            var currentToken = tokens[i];

            if (!currentToken.StartsWith(argIndicator))
                continue;

            if (i == tokens.Length - 1)
            {
                args.Add(currentToken.Substring(argIndicator.Length));
                break;
            }

            args.Add($"{currentToken} ");
        }

        return args.ToArray();
    }

    //private static string GetParameters(string[] tokens, string[] paramIndicator)
    //{
    //    if (!paramIndicator.Any())
    //        throw new ArgumentNullException(nameof(paramIndicator));

    //    if (tokens.Length <= 1)
    //        return null;

    //    var sb = new StringBuilder();

    //    for (int i = 1; i < tokens.Length;)
    //    {
    //        var currentToken = tokens[i];

    //        if (!currentToken.IsStartWithIndicator(paramIndicator))
    //        {
    //            ++i;
    //            continue;
    //        }

    //        if (i == tokens.Length - 1)
    //            break;

    //        var nextToken = tokens[i + 1];

    //        if (nextToken.IsStartWithIndicator(paramIndicator))
    //        {
    //            ++i;
    //            continue;
    //        }

    //        sb.Append($"{currentToken} {nextToken} ");
    //        i += 2;
    //    }

    //    return sb.ToString();
    //}
}
