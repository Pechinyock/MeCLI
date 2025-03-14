using System;

namespace Me;

internal sealed class ConsoleLogger : ILogger
{
    private Verbosity _verbosity;
    private const ConsoleColor DEFAULT_COSOLE_TEXT_COLOR = ConsoleColor.White;

    public void ChangeBgColor(ConsoleColor color) 
    {
        Console.BackgroundColor = color;
    }

    #region ILogger
    public void SetVerbosity(Verbosity value) => _verbosity |= value;

    public void Trace(string msg)
    {
        if (!Guard(msg, Verbosity.Trace))
            return;
        PrintColored(msg, ConsoleColor.Gray);
    }

    public void Info(string msg)
    {
        if (!Guard(msg, Verbosity.Info))
            return;
        PrintColored(msg, ConsoleColor.Green);
    }

    public void Warn(string msg)
    {
        if (!Guard(msg, Verbosity.Warn))
            return;
        PrintColored(msg, ConsoleColor.Yellow);
    }

    public void Error(string msg)
    {
        if (!Guard(msg, Verbosity.Error))
            return;
        PrintColored(msg, ConsoleColor.Red);
    }
    #endregion

    private static void PrintColored(string msg, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(msg);
        Console.ForegroundColor = DEFAULT_COSOLE_TEXT_COLOR;
    }

    private bool Guard(string msg, Verbosity level) 
    {
        if (!_verbosity.HasFlag(level)) 
            return false;
        if (String.IsNullOrEmpty(msg))
            return false;

        return true;
    }
}
