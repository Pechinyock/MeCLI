namespace Me;

internal sealed class Create : MeCommandBase
                             , IArgumented
                             , IParametrized
                             , IDescribed
{
    private static readonly string[] _argsNames = new string[]
    {
        "list"
    };

    private static readonly Dictionary<string, string> _argsWithDescription = new()
    {
        { /* list */ _argsNames[0], "list all of things that could be created" }
    };

    private static readonly Dictionary<string, Action> _argsActions = new()
    {
        { /* list */ _argsNames[0], CreateList.Do }
    };

    private static readonly string[] _requiredParams = new string[]
    {
        "type",
        "name",
        "language"
    };

    private static readonly string[] _optionalParams = new string[]
    {
        "tempate",
        "mode"
    };

    private static readonly Dictionary<string, string> _paramsWithDescription = new()
    {
        { /* type */ _requiredParams[0], "specify type of creating thing. project / template etc..." },
        { /* name */ _requiredParams[1], "specify name of creating thing." },
        { /* language */ _requiredParams[2], "specifing using programming language"},
        { /* template */ _optionalParams[0], "create someting from template" },
        { /* mode */ _optionalParams[1], $"specufying creating mode: immediately or step-by-step. Default is: immediately"}
    };

    internal static readonly string[] AvailableTypesToCreate = new string[]
    {
        "project", "template"
    };

    private static readonly Dictionary<string, Action<Dictionary<string, string>>> _paramActions = new() 
    {
        { /* projcet */ AvailableTypesToCreate[0], CreateProject.Do },
        { /* template */ AvailableTypesToCreate[1], CreateTemplate.Do },
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

        if (_passedArguments.Length > 1)
        {
            Print.Error("Too many arguments");
            return false;
        }

        if (_passedParameters.Count > 0 && _passedArguments.Length > 0)
        {
            Print.Error("You can't specify parameters and arguments at the same time!");
            return false;
        }

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

            foreach (var required in _requiredParams)
            {
                if (!_passedParameters.ContainsKey(required))
                {
                    Print.Error($"You have to specify required parameter: {GetParameterIndicator()}{required}");
                    return false;
                }
            }
        }

        var typeParameter = _passedParameters[/* type */_requiredParams[0]];
        if (!AvailableTypesToCreate.Contains(typeParameter))
        {
            Print.Error($"Unknown type: {typeParameter}");
            Print.Warn($"Here the list of things that you can create with 'me' <3");
            var redirectToList = new Create();
            redirectToList._passedArguments = new string[]{ /* list */_argsNames[0] };
            redirectToList.Execute();
            return false;
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

        if (IsArgsSpecified()) 
        {
            var arg = _passedArguments[0];
            if (arg is not null)
                _argsActions[arg].Invoke();
        }
        if (IsParamsSpecified()) 
        {
            var parameters = _passedParameters;
            var creatingThingType = _passedParameters[/* type */_requiredParams[0]];
            _paramActions[creatingThingType].Invoke(_passedParameters);
        }
    }

    private bool IsArgsSpecified() => _passedArguments is not null && _passedArguments.Length != 0;
    private bool IsParamsSpecified() => _passedParameters is not null && _passedParameters.Count != 0;
}
