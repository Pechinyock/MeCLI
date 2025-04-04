using System.Diagnostics;

namespace Me;

internal sealed class Parrot : IConsolePrinter
{
    private const ConsoleColor DEFAULT_COLOR = ConsoleColor.White;

    public void ChangeWirittenRowText(string newText, int rowIndex, ConsoleColor color)
    {
        var returnBackPos = Console.GetCursorPosition();
        Console.SetCursorPosition(0, rowIndex);
        PrintColored(newText, color);
        Console.SetCursorPosition(returnBackPos.Left, returnBackPos.Top);
    }

    public int GetCurrentTopPossition() => Console.GetCursorPosition().Top;

    public void Print(string message)
    {
        Console.WriteLine(message);
    }

    public void Print(ConsoleTable table)
    {
        Debug.Assert(table is not null);
        Print(table.ToString());
    }

    public void Print(InteractivePannel pannel) 
    {
        Debug.Assert(pannel is not null);
        pannel.DrawInteractive(this);
    }

    public void PrintColored(string message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ForegroundColor = DEFAULT_COLOR;
    }
}
