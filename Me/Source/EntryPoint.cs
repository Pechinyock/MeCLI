using Me;

internal static class EntryPoint
{
    /* [TODO]
     * Printer as seporate class coz logger != printer
     * Last column width has to be his width - 1 coz it is always contains new line character
     * Table is a part of printer, not help command
     * Include tests write some of them
     * Validate stage has to be implemented
     * Args and params info inside cmd is a dictionary key - arg\prarm alias value is description for help command
     * Create interface ISubcommanded that will hanle command that can do sub commands:
     *  - like help <cmd name> or create <template\notification\database_script>
     *  - sub command ALWAYS second token after main command
     */

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