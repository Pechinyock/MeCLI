using System.Diagnostics;

namespace Me;

internal sealed class Parrot : IConsolePrinter
{
    private const ConsoleColor DEFAULT_COLOR = ConsoleColor.White;

    public void Print(string message)
    {
        Console.WriteLine(message);
    }

    public void PrintColored(string message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ForegroundColor = DEFAULT_COLOR;
    }

    public void PrintTable(ConsoleTable table)
    {
        Debug.Assert(table != null);
        Print(table.ToString());
    }
}
