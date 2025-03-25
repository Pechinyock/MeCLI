namespace Me;

public interface IConsolePrinter
{
    public void Print(string message);
    public void PrintColored(string message, ConsoleColor color);
    public void PrintTable(ConsoleTable table);
}
