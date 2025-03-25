using System.Diagnostics;

namespace Me;

internal abstract class Log
{
#if DEBUG
    private static bool _isInit = false;
#endif

    private static ILogger _logger;

    public static void InitializeLogger(ILogger log) 
    {
#if DEBUG
        Debug.Assert(!_isInit, "Log was initialized multiple times!");
        _isInit = true;
#endif
        _logger = log;
    }

    public static void Trace(string message) => _logger.Trace(message);
    public static void Info(string message) => _logger.Info(message);
    public static void Warn(string message) => _logger.Warn(message);
    public static void Error(string message) => _logger.Error(message);
}
