namespace Me;

public interface IPipelineContext
{
    string[] GetSourceInput();
    MeCommandBase GetExecutingCommand();
    void SetExecutingCommand(MeCommandBase cmd);
}
