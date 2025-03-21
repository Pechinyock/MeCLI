namespace Me;

internal abstract class Print
{
    private static readonly IConsolePrinter printer = new Parrot();

    public static void String(string message) => printer.Print(message);
}
