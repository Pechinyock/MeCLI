using Me;

internal static class EntryPoint
{
    /* [TODO]
     * ISubcommanded interface
     * collapse everyting to application class Mian should loks like App.Run(args);
     * inside App class specifying special things for OS linux\windows:
     *  - condidtional compilation:
     *       -- split configuration(Debug-windows, Deubg-linux, Release-windows, Release-lnux);
     *       -- IFileSystem is a first that should be splidet by this condidonal compilation;
     *       -- Wrap IFileSystem with Lazy;
     */

    public static void Main(string[] args)
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