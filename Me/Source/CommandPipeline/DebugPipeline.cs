#if DEBUG

namespace Me;

internal sealed class DebugPipeline : PipelineBase
{
    public DebugPipeline(IPipelineContext context)
    {
        Stages = new StageBase[]
        {
            new Find(context),
            new Parse(context),
            new Validate(context),
            new Execute(context)
        };
    }

    public override void PushThrough(string[] input)
    {
        foreach (var stage in Stages) 
        {
            stage.OnFailure += ErrorHandler;
            if (!stage.Proceed())
                break;
        }
    }

    private void ErrorHandler(string reason) 
    {
        Print.Error(reason);
    }
}

#endif