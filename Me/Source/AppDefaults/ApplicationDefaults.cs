using System.Diagnostics;

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
        log.SetVerbosity(Verbosity.Error | Verbosity.Info | Verbosity.Warn | Verbosity.Trace);
        return log;
    }

    public static TableDisplaySettings GetTableDisplaySettings(int columnsCount)
    {
        Debug.Assert(columnsCount > 0);

        var consoleWidth = Console.WindowWidth;

        int equalPrecent = 100 / columnsCount;
        var columnsWidth = new int[columnsCount];

        for (int i = 0; i < columnsCount; ++i)
            columnsWidth[i] = equalPrecent;

        return new TableDisplaySettings()
        {
            ColumnsWidthPrecents = columnsWidth,
            PrintHeader = true,
            ColumnSeporator = '|',
            RowSeporator ='-'
        };
    }
}
