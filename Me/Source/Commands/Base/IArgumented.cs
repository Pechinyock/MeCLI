namespace Me;

internal interface IArgumented
{
    public void SetArguments(string[] value);

    public string[] GetPassedArguments();

    public string GetArgumentIndicator();

    public string[] GetArgsWithdescription();

    public Dictionary<string, string> GetAvailableArgs();
}
