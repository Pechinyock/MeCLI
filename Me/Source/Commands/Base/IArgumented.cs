namespace Me;

public interface IArgumented
{
    void SetArguments(string[] value);

    string[] GetPassedArguments();

    string GetArgumentIndicator();

    string[] GetArgsWithdescription();

    Dictionary<string, string> GetAvailableArgs();
}
