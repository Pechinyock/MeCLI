namespace Me;

internal sealed class Tools : MeCommandBase
                            , IArgumented
                            , IDescribed
{
    private string[] _pasedArguments;
    public override string Alias => "tools";

    public override CmdTypeEnumFlag GetTypes => CmdTypeEnumFlag.Described | CmdTypeEnumFlag.Argumented;

    private readonly string[] _namesOfRequiredTools = new string[]
    {
        "dotnet",
        "git",
        "docker"
    };

    private readonly string[] _availableArgumetns = new string[] 
    {
        "list"
    };

    public override void Execute()
    {
        Console.WriteLine("Executing tools");
    }

    public string GetDescription() => "It is tools description";

    public Dictionary<string, string> GetParamsArgsDescription() => new Dictionary<string, string>() 
    {
        { _availableArgumetns[0], "prints all of available tools" }
    };

    public void SetArguments(string[] value) => _pasedArguments = value;

    public string GetArgumentIndicator() => CommandsDefaults.ARGS_INDICATOR;

}
