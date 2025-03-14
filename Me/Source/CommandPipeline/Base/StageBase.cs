namespace Me;

public abstract class StageBase
{
    protected StageBase(IPipelineContext context) => _context = context;

    protected readonly IPipelineContext _context;

    public abstract event Action<string> OnFailure;

    public abstract bool Proceed();
}
