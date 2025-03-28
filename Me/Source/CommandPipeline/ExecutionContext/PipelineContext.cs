using System.Diagnostics;

namespace Me;

internal sealed class PipelineContext : IPipelineContext
{
    private readonly string[] _sourceInput;
    private MeCommandBase _executingCommand;
    private bool _isCommandSet = false;

    public PipelineContext(string[] input)
    {
        if (input is null && input.Length == 0)
            Print.Error("Couldn't execute empty command");
        _sourceInput = input;
    }
    public MeCommandBase GetExecutingCommand() => _executingCommand;
    public string[] GetSourceInput() => _sourceInput;

    public void SetExecutingCommand(MeCommandBase cmd)
    {
        Debug.Assert(cmd != null);
        Debug.Assert(!_isCommandSet);

        if (_isCommandSet)
            return;

        _executingCommand = cmd;
        _isCommandSet = true;
    }
}
