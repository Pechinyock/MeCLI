using System.Diagnostics;

namespace Me;

public sealed class InteractivePannel
{
    private readonly MultiStepOperation _operation;
    private readonly string[] _rows;

    public InteractivePannel(MultiStepOperation operation)
    {
        Debug.Assert(operation is not null);
        Debug.Assert(operation.StepsCount > 0);

        var rows = new string[operation.StepsCount];

        _operation = operation;

        for (int i = 0; i < rows.Length; i++)
        {
            rows[i] = operation[i].GetName();
        }
        _rows = rows;
    }

    public void DrawInteractive(IConsolePrinter printer)
    {
        var tab = "   ";
        var arrow = " ->";
        var failMark = "[X]";
        var successMark = "[V]";

        printer.Print(_operation.Title);
        foreach (var row in _rows)
        {
            printer.Print($"{tab}{row}");
        }

        var positionTop = printer.GetCurrentTopPossition();

        _operation.OnStepStarting += (index, text) =>
        {
            var rowIndex = positionTop - _rows.Length + index;
            printer.ChangeWirittenRowText($"{arrow} {text}", rowIndex, ConsoleColor.Yellow);
        };

        _operation.OnStepByStepWaited += () =>
        {
            printer.ChangeWirittenRowText($"Press enter to continue", positionTop, ConsoleColor.Yellow);
            Console.ReadLine();
        };

        _operation.OnStepCompleted += (index, text) =>
        {
            var rowIndex = positionTop - _rows.Length + index;
            printer.ChangeWirittenRowText($"{successMark} {text}", rowIndex, ConsoleColor.Green);
        };

        _operation.OnStepFailed += (index, text) =>
        {
            var rowIndex = positionTop - _rows.Length + index;
            printer.ChangeWirittenRowText($"{failMark} {text}", rowIndex, ConsoleColor.Red);
        };
    }
}
