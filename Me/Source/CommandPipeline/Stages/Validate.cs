using System.Diagnostics;

namespace Me;

internal sealed class Validate : StageBase
{
    public Validate(IPipelineContext context) : base(context) {}

    public override event Action<string> OnFailure;

    public override bool Proceed()
    {
        var cmd = _context.GetExecutingCommand();
        Debug.Assert(cmd != null);
        return cmd.Validate();
    }
}
