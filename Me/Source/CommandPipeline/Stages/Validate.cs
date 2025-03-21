using System.Diagnostics;

namespace Me;

internal sealed class Validate : StageBase
{
    public Validate(IPipelineContext context) : base(context) {}

    public override event Action<string> OnFailure;

    public override bool Proceed()
    {
        var cmd = _context.GetExecutingCommand();
        Debug.Assert(cmd != null);

        if (cmd.IsArgumented())
        {
            if(!ValidateArgs(cmd))
                return false;
        }

        if (cmd.IsParametrized()) 
        {
            if (!ValidateParameters(cmd))
                return false;
        }

        return true;
    }

    private bool ValidateArgs(MeCommandBase cmd) 
    {
        var argumented = cmd as IArgumented;
        Debug.Assert(argumented != null);
        var availableArgs = argumented.GetAvailableArgs();
        var passedArgs = argumented.GetPassedArguments();
        if (passedArgs is null || passedArgs.Length == 0)
            return true;

        foreach (var arg in passedArgs) 
        {
            if (!availableArgs.ContainsKey(arg))
            {
                OnFailure?.Invoke($"Unknown argument: {argumented.GetArgumentIndicator()}{arg}");
                return false;
            }
        }
        return true;
    }

    private bool ValidateParameters(MeCommandBase cmd) 
    {
        var parametrized = cmd as IParametrized;
        Debug.Assert(parametrized != null);
        var availableParameters = parametrized.GetAvailableParameters();
        var passedParameters = parametrized.GetPassedParameters();

        if (passedParameters is null || passedParameters.Count == 0)
            return true;

        foreach (var parameter in passedParameters)
        {
            if (!availableParameters.ContainsKey(parameter.Key))
            {
                OnFailure?.Invoke($"Unknown parameter: {parametrized.GetParameterIndicator()}{parameter.Key}");
                return false;
            }
        }

        return true;
    }
}
