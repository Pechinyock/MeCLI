using System.Text;
using System.Diagnostics;

namespace Me;

internal sealed class Parse : StageBase
{
    public Parse(IPipelineContext context) : base(context) { }

    public override event Action<string> OnFailure;

    public override bool Proceed()
    {
        var cmdToParse = _context.GetExecutingCommand();
        var sourceInput = _context.GetSourceInput();

        Debug.Assert(sourceInput != null && sourceInput.Length > 0);
        Debug.Assert(cmdToParse != null);

        if (cmdToParse.IsArgumented())
            FillArguments(sourceInput, cmdToParse);

        if (cmdToParse.IsParametrized())
            FillParameters(sourceInput, cmdToParse);

        if (cmdToParse.IsExternal())
            FillExternal(sourceInput, cmdToParse);

        if (cmdToParse.IsSubcommanded())
            FillSubcommands(sourceInput, cmdToParse);

        return true;
    }

    private void FillSubcommands(string[] input, MeCommandBase foundedInRegistry)
    {
        var subcommanded = foundedInRegistry as ISubcommanded;
        Debug.Assert(subcommanded is not null, $"{foundedInRegistry.Alias} marked as subcommanded but doesn't implement {nameof(ISubcommanded)}");
        var subcommands = GetSubcommands(input, foundedInRegistry);
        subcommanded.SetSubcommand(subcommands);
    }

    private void FillArguments(string[] input, MeCommandBase foundedInRegistry)
    {
        var argumented = foundedInRegistry as IArgumented;
        Debug.Assert(argumented is not null, $"{foundedInRegistry.Alias} marked as argumented but doesn't implement {nameof(IArgumented)}");

        var indicators = argumented.GetArgumentIndicator();
        var argumetns = GetArguments(input, indicators);
        argumented.SetArguments(argumetns);
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

            if (!IsIndicator(currentToken, argIndicator))
                continue;

            args.Add(currentToken.Substring(argIndicator.Length));
        }

        return args.ToArray();
    }

    private string[] GetSubcommands(string[] tokens, MeCommandBase cmd)
    {
        if (tokens.Length <= 1)
            return null;

        var paramIndicator = String.Empty;

        var subcommands = new List<string>(tokens.Length - 1);
        var checkForParameterValue = cmd.IsParametrized();
        if (checkForParameterValue) 
        {
            var parametrized = cmd as IParametrized;
            paramIndicator = parametrized.GetParameterIndicator();
        }

        for (int i = 1; i < tokens.Length; ++i)
        {
            if (String.IsNullOrEmpty(tokens[i]))
                continue;

            var currentToken = tokens[i];
            var character = currentToken[0];
            if (!Char.IsLetter(character))
                continue;

            if (checkForParameterValue) 
            {
                if (i > 1)
                {
                    var prevToken = tokens[i - 1];
                    var prevFirstChar = prevToken[0];
                    if (!Char.IsLetter(prevFirstChar))
                        continue;
                }
            }

            subcommands.Add(currentToken);
        }

        return subcommands.ToArray();
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
        var parametrized = foundedInRegistry as IParametrized;
        Debug.Assert(parametrized is not null, $"{foundedInRegistry.Alias} marked as paramtrized but doesn't implement {nameof(IParametrized)}");

        var parameters = GetParameters(input, parametrized.GetParameterIndicator());

        parametrized.SetParameters(parameters);
    }

    private Dictionary<string, string> GetParameters(string[] tokens, string paramIndicator)
    {
        if (tokens.Length <= 1)
            return null;

        var result = new Dictionary<string, string>();
        for (int i = 1; i < tokens.Length; ++i)
        {
            var currentToken = tokens[i];
            if (String.IsNullOrEmpty(currentToken))
                continue;

            if (!IsIndicator(currentToken, paramIndicator))
                continue;

            var key = currentToken.Substring(paramIndicator.Length);
            var nextIndex = i + 1;
            string value = nextIndex < tokens.Length
                ? tokens[nextIndex]
                : null;

            if (result.ContainsKey(key))
            {
                OnFailure?.Invoke($"Parameter with the same name was provided: {currentToken}");
                return null;
            }
            result.Add(key, value);
        }

        return result;
    }

    private static bool IsIndicator(string token, string indicator)
    {
        Debug.Assert(!String.IsNullOrEmpty(token));

        var tokenToLower = token.ToLower();
        var indicatorToLower = indicator.ToLower();

        for (int i = 0; i < indicator.Length; i++)
        {
            if (indicatorToLower[i] != tokenToLower[i])
                return false;
        }

        var nextToIndicatorChar = token[indicator.Length];
        var result = Char.IsLetter(nextToIndicatorChar);

        return result;
    }
}
