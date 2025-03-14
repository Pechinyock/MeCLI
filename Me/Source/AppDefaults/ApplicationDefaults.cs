namespace Me;

internal static class ApplicationDefaults
{
#if DEBUG
    public static ILogger GetDebugLogger() 
    {
        var log = new ConsoleLogger();
        log.SetVerbosity(Verbosity.All);
        return log;
    }
#endif

    public static ILogger GetConsoleReleaseLogger() 
    {
        var log = new ConsoleLogger();
        log.SetVerbosity(Verbosity.Error | Verbosity.Info | Verbosity.Trace);
        return log;
    }
}
