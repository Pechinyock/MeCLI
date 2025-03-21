namespace Me;

internal sealed class Parrot : IConsolePrinter
{
    public void Print(string message)
    {
        Console.WriteLine(message);
    }
}
