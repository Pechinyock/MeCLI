
namespace Me;

internal sealed class Tools : MeCommandBase
                            , IArgumented
                            , IDescribed
{
    private string[] _pasedArguments;

    private Dictionary<string, string> _argsWithDescription = new() 
    {
        { "list", "list all available tools" }
    };

    public override string Alias => "tools";
    public override bool Validate() => true;
    public override CmdTypeEnumFlag GetTypes => CmdTypeEnumFlag.Described | CmdTypeEnumFlag.Argumented;

    private readonly string[] _namesOfRequiredTools = new string[]
    {
        "dotnet",
        "git",
        "docker"
    };

    public override void Execute()
    {
        Console.WriteLine("Executing tools");
    }

    public string GetDescription() => "It is tools description It is tools description It is tools description It is tools description It is tools description It is tools description It is tools description It is tools description It is tools description It is tools description It is tools description It is tools description It is tools description It is tools description It is tools description It is tools description It is tools description";

    public void SetArguments(string[] value) => _pasedArguments = value;
    public string[] GetPassedArguments() => _pasedArguments;
    public string GetArgumentIndicator() => CommandsDefaults.ARGS_INDICATOR;
    public string[] GetArgsWithdescription() => CommandsDefaults.GetArgsOrParamsDescriptionDefault(_argsWithDescription, GetArgumentIndicator());
    public Dictionary<string, string> GetAvailableArgs() => _argsWithDescription;
}
