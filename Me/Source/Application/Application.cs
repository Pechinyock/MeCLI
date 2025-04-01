namespace Me;

internal class Application
{
    internal static Lazy<IFilesManager> FilesManager => new Lazy<IFilesManager>(() => new FilesManager());

    public void Run(string[] args)
    {
#if DEBUG
        Print.String("Running in debug mode");
        Log.InitializeLogger(ApplicationDefaults.GetDebugLogger());
        var pretendInput = Console.ReadLine();
        var argsPretender = pretendInput.Split(' ');
        var debugContext = new PipelineContext(argsPretender);
        var debugPipe = new DebugPipeline(debugContext);
        debugPipe.PushThrough(null);
        return;
#endif
        Log.InitializeLogger(ApplicationDefaults.GetConsoleReleaseLogger());
        var context = new PipelineContext(args);
        var releasePipe = new ReleasePipeline(context);
        releasePipe.PushThrough(args);
    }

}
