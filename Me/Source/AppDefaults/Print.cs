namespace Me;

internal abstract class Print
{
    private static readonly IConsolePrinter printer = new Parrot();

    public static void String(string message) => printer.Print(message);
    public static void Error(string messgage) => printer.PrintColored(messgage, ConsoleColor.Red);
    public static void Info(string message) => printer.PrintColored(message, ConsoleColor.Green);
    public static void Warn(string message) => printer.PrintColored(message, ConsoleColor.Yellow);
    public static void Table(ConsoleTable table) => printer.Print(table);
    public static void InteractivePannel(InteractivePannel pannel) => printer.Print(pannel);
}
