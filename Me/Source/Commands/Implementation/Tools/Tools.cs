
namespace Me;

internal sealed class Tools : MeCommandBase
                            , IArgumented
                            , IDescribed
{
    private string[] _passedArguments;
    private static readonly string _defaultInstallChecker = "--version";


    private static string[] _argumentsAliases = new string[]
    {
       "check"
    };

    private Dictionary<string, string> _argsWithDescription = new() 
    {
        { _argumentsAliases[0], "check installation of requred external tools" }
    };

    private static Dictionary<string, Action> _argActionMap = new()
    {
        { _argumentsAliases[0], OnCheck }
    };

    public override string Alias => "tools";
    public override CmdTypeEnumFlag GetTypes => CmdTypeEnumFlag.Described | CmdTypeEnumFlag.Argumented;
    public string GetDescription() => "Set of external tools that required for develop with 'Me'";
    public void SetArguments(string[] value) => _passedArguments = value;
    public string[] GetPassedArguments() => _passedArguments;
    public string GetArgumentIndicator() => CommandsDefaults.ARGS_INDICATOR;
    public string[] GetArgsWithdescription() => CommandsDefaults.GetArgsOrParamsDescriptionDefault(_argsWithDescription, GetArgumentIndicator());
    public Dictionary<string, string> GetAvailableArgs() => _argsWithDescription;

    private static readonly string[] _namesOfRequiredTools = new string[]
    {
        "dotnet",
        "git",
        "docker",
    };

    private static readonly Dictionary<string, string> _argumentChecker = new()
    {
        { _namesOfRequiredTools[0], _defaultInstallChecker },
        { _namesOfRequiredTools[1], _defaultInstallChecker },
        { _namesOfRequiredTools[2], _defaultInstallChecker },
    };

    public override bool Validate() 
    {
        if(_passedArguments is null || _passedArguments.Length == 0)
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

    public override void Execute()
    {
        if (_passedArguments is null || _passedArguments.Length == 0) 
        {
            CommandsDefaults.RedirectToHelp(Alias);
            return;
        }

        foreach (var arg in _passedArguments) 
        {
            var action = _argActionMap[arg];

            if (action != null)
                action?.Invoke();
        }
    }

    private static void OnCheck() 
    {
        var externalCmd = new ExternalCommandWrapper();

        foreach (var tool in _namesOfRequiredTools) 
        {
            externalCmd.SetProgramName(tool);
            var checkArg = _argumentChecker[tool];
            externalCmd.SetArguments(checkArg);

            externalCmd.Execute();

            while (!externalCmd.IsCommandDone())
            {
                Task.Delay(1000);
            }

            var result = externalCmd.GetOutputRestlt();
            if (result == "error")
            {
                Print.Error($"The tool: '{tool}' is not available." +
                    $" It might be not installed or hasn't be added to Path variable");

                continue;
            }
            
            Print.Info($"{tool}: {result}");
        }
    }


}
