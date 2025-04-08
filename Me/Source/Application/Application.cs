namespace Me;

internal class Application
{
    internal static IFilesManager FilesManager => _filesManager.Value;
    internal static ITextValidator TextValidator => _textValidator.Value;
    internal static IConfigLoader ConfigLoader => _configLoader.Value;

    private static Lazy<IFilesManager> _filesManager => new Lazy<IFilesManager>(() => new FilesManager());
    private static Lazy<ITextValidator> _textValidator => new Lazy<ITextValidator>(() => new RegexValidator());
    private static Lazy<IConfigLoader> _configLoader => new Lazy<IConfigLoader>(() => new ConfigLoader());

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
