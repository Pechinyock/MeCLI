namespace Me;

internal sealed class Create : MeCommandBase
                             , IArgumented
                             , IParametrized
                             , IDescribed
{
    private static readonly Dictionary<string, string> _argsWithDescription = new()
    {
        { "list", "list all of things that could be created"}
    };

    private static readonly Dictionary<string, string> _paramsWithDescription = new() 
    {
        { "type", "specify type of creating thing. 'project / template etc..."},
        { "name", "specify name of creating thing."},
        { "template", "create someting from template" },
    };

    private string[] _passedArguments;

    private Dictionary<string, string> _passedParameters;

    public override string Alias => "create";
    public override CmdTypeEnumFlag GetTypes => CmdTypeEnumFlag.Argumented
                                              | CmdTypeEnumFlag.Parzmetrized
                                              | CmdTypeEnumFlag.Described;

    public void SetArguments(string[] value) => _passedArguments = value;
    public string[] GetPassedArguments() => _passedArguments;
    public string GetArgumentIndicator() => CommandsDefaults.ARGS_INDICATOR;
    public string[] GetArgsWithdescription() => CommandsDefaults.GetArgsOrParamsDescriptionDefault(_argsWithDescription, GetArgumentIndicator());
    public Dictionary<string, string> GetAvailableArgs() => _argsWithDescription;

    public string GetParameterIndicator() => CommandsDefaults.PARAM_INDICATOR;
    public string[] GetParamsWithDescription() => CommandsDefaults.GetArgsOrParamsDescriptionDefault(_paramsWithDescription, GetParameterIndicator());
    public void SetParameters(Dictionary<string, string> value) => _passedParameters = value;
    public Dictionary<string, string> GetAvailableParameters() => _paramsWithDescription;
    public Dictionary<string, string> GetPassedParameters() => _passedParameters;

    public string GetDescription() => "Bassicly creates something";
    public override bool Validate() 
    {
        if (!IsArgsSpecified() && !IsParamsSpecified())
            return true;

        foreach (var arg in _passedArguments)
        {
            if (!_argsWithDescription.ContainsKey(arg))
            {
                Print.Error($"Unknown argument: {GetArgumentIndicator()}{arg}");
                return false;
            }
        }

        foreach (var param in _passedParameters) 
        {
            if (!_paramsWithDescription.ContainsKey(param.Key)) 
            {
                Print.Error($"Unknown parameter: {GetParameterIndicator()}{param.Key}");
                return false;
            }
        }

        return true;
    }

    public override void Execute()
    {
        if (!IsArgsSpecified() && !IsParamsSpecified())
        {
            CommandsDefaults.RedirectToHelp(Alias);
            return;
        }
    }

    private bool IsArgsSpecified() => _passedArguments is not null && _passedArguments.Length != 0;
    private bool IsParamsSpecified() => _passedParameters is not null && _passedParameters.Count != 0;
}
