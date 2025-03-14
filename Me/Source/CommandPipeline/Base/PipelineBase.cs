namespace Me;

public abstract class PipelineBase
{
    public StageBase[] Stages { get; protected set; }

    public abstract void PushThrough(string[] input);
}
