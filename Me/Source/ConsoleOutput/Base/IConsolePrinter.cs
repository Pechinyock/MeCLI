namespace Me;

public interface IConsolePrinter
{
    void Print(string message);
    void PrintColored(string message, ConsoleColor color);
    void Print(ConsoleTable table);
    void Print(InteractivePannel pannel);
    void ChangeWirittenRowText(string newText, int rowIndex, ConsoleColor color);
    int GetCurrentTopPossition();
}
