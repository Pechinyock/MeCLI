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
        { "template", "create someting from template" }
    };

    private string[] _passedArguments;

    private Dictionary<string, string> _passedParameters;

    public override string Alias => "create";

    public override CmdTypeEnumFlag GetTypes => CmdTypeEnumFlag.Argumented 
                                              | CmdTypeEnumFlag.Parzmetrized
                                              | CmdTypeEnumFlag.Described;

    public override void Execute()
    {
        int i = 0;
    }

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

}
