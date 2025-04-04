namespace Me;

internal sealed class Create : MeCommandBase
                             , IArgumented
                             , IParametrized
                             , IDescribed
                             , ISubcommanded
{
    private static readonly string[] _argsNames = new string[]
    {
        "list",
        "step-by-step"
    };

    private static readonly Dictionary<string, string> _argsWithDescription = new()
    {
        { /* list */ _argsNames[0], "list all of things that could be created" },
        { /* step-by-step */ _argsNames[1], "enables interactive mode" }
    };

    private static readonly Dictionary<string, Action> _argsActions = new()
    {
        { /* list */ _argsNames[0], CreateList.Do }
    };

    private static readonly string[] _requiredParams = new string[]
    {
        "type",
        "name",
        "language",
        "path"
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
        { /* language */ _requiredParams[3], "path where create"},
        { /* mode */ _optionalParams[1], $"specufying creating mode: immediately or step-by-step. Default is: immediately"}
    };

    private static readonly string[] _availableSubcommands = new string[]
    {
        "application",
        "project",
    };

    internal static readonly Dictionary<string, string> AvailableSubcommandsWithDescription = new()
    {
        { /* application */ _availableSubcommands[0], "create new application" }
    };

    private static readonly Dictionary<string, Action<Dictionary<string, string>, string[]>> _subcommandActions = new()
    {
        { /* application */ _availableSubcommands[0], CreateApplication.Do },
    };

    private string[] _passedSubcommands;
    private string[] _passedArguments;
    private Dictionary<string, string> _passedParameters;

    public override string Alias => "create";
    public override CmdTypeEnumFlag GetTypes => CmdTypeEnumFlag.Argumented
                                              | CmdTypeEnumFlag.Parzmetrized
                                              | CmdTypeEnumFlag.Described
                                              | CmdTypeEnumFlag.Subcommanded;

    public void SetSubcommand(string[] subCommands) => _passedSubcommands = subCommands;
    public Dictionary<string, string> GetAvailableSubcommands() => AvailableSubcommandsWithDescription;

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
        if (!IsArgsValid())
            return false;

        if (!IsSubcommandValid())
            return false;

        return true;
    }

    public override void Execute()
    {
        if (!IsArgsSpecified() && !IsSubcommandSpecified())
        {
            CommandsDefaults.RedirectToHelp(Alias);
            return;
        }

        if (IsArgsSpecified() && !IsSubcommandSpecified())
        {
            var arg = _passedArguments[0];
            if (arg is not null)
                _argsActions[arg].Invoke();
            return;
        }

        var parameters = _passedParameters;
        var creatingThingType = _passedSubcommands[0];
        _subcommandActions[creatingThingType].Invoke(_passedParameters, _passedArguments);
    }

    private bool IsArgsSpecified() => _passedArguments is not null && _passedArguments.Length != 0;
    private bool IsParamsSpecified() => _passedParameters is not null && _passedParameters.Count != 0;
    private bool IsSubcommandSpecified() => _passedSubcommands is not null && _passedSubcommands.Length != 0;

    private bool IsArgsValid()
    {
        if (!IsArgsSpecified())
            return true;

        foreach (var arg in _passedArguments)
        {
            if (!_argsWithDescription.ContainsKey(arg))
            {
                Print.Error($"Unknown argument: {GetArgumentIndicator()}{arg}");
                return false;
            }
        }

        return true;
    }

    private static bool IsCreateAppValid(Dictionary<string, string> passedParams, string[] passedArgs)
    {
        if (passedArgs is not null && passedArgs.Length > 0) 
        {
            var availabelArgs = new Dictionary<string, bool>() 
            {
                { "step-by-step", false }
            };
            var unknown = new List<string>(passedArgs.Length);
            foreach (var arg in passedArgs) 
            {
                if (!availabelArgs.ContainsKey(arg)) 
                    unknown.Add(arg);
            }
            var argsValid = true;
            foreach (var u in unknown) 
            {
                Print.Error($"Unknown argument: {u}");
                argsValid = false;
            }
            if (!argsValid)
                return false;
        }

        var reqiuredForApp = new Dictionary<string, bool>()
        {
            { "path", false },
            { "name", false },
            { "type", false }
        };

        if (passedParams == null)
            return false;

        foreach (var parameter in passedParams) 
        {
            if (reqiuredForApp.ContainsKey(parameter.Key)) 
            {
                reqiuredForApp[parameter.Key] = true;
            }
        }

        bool valid = true;
        foreach (var notFilled in reqiuredForApp) 
        {
            if (!notFilled.Value) 
            {
                valid = false;
                Print.Error($"Required parameter is not filled. Parameter name: {notFilled.Key}");
            }
        }

        return valid;
    }

    private static readonly Dictionary<string, Func<Dictionary<string, string>, string[], bool>> _subcommandsParamsValidationMap = new()
    {
        { _availableSubcommands[0],  IsCreateAppValid }
    };


    private bool IsSubcommandValid() 
    {
        if (!IsSubcommandSpecified())
            return true;

        if (_passedSubcommands.Length > 1)
        {
            Print.Error("Too many subcommands");
            return false;
        }

        if (!AvailableSubcommandsWithDescription.ContainsKey(_passedSubcommands[0]))
        {
            Print.Error($"Unknown subcommand: {_passedSubcommands[0]}");
            return false;
        }

        if (IsParamsSpecified()) 
        {
            foreach (var parameter in _passedParameters) 
            {
                if (!_paramsWithDescription.ContainsKey(parameter.Key)) 
                {
                    Print.Error($"Unknown parameter: {parameter.Key}");
                    return false;
                }
            }
        }

        var validateFunc = _subcommandsParamsValidationMap[_passedSubcommands[0]];
        var validationResult = validateFunc.Invoke(_passedParameters, _passedArguments);
        return validationResult;
    }

}
