using System.Diagnostics;

namespace Me;

internal sealed class Find : StageBase
{
    public Find(IPipelineContext context) : base(context) {}

    public override event Action<string> OnFailure;

    public override bool Proceed()
    {
        var sourceString = _context.GetSourceInput();
        var requestedAlias = sourceString[0];
        var foundCmd = Librarian.Request(requestedAlias);

        if (foundCmd is null) 
        {
            OnFailure?.Invoke($"Command: {requestedAlias} not found");
            return false;
        }
        _context.SetExecutingCommand(foundCmd);
        return true;
    }

}