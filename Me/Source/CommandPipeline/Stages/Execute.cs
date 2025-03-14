using System.Diagnostics;

namespace Me;

internal class Execute : StageBase
{
    public Execute(IPipelineContext context) : base(context) {}

    public override event Action<string> OnFailure;

    public override bool Proceed()
    {
        var cmd = _context.GetExecutingCommand();
        Debug.Assert(cmd != null);
        cmd.Execute();
        return true;
    }
}
