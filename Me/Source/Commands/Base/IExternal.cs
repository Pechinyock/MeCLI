namespace Me;

public interface IExternal
{
    void SetArguments(string value);
    void SetProgramName(string value);
    bool IsCommandDone();
    string GetOutputRestlt();
}
