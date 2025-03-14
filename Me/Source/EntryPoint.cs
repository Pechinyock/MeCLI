using Me;

internal static class EntryPoint
{
    public static void Main(string[] args)
    {
#if DEBUG
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