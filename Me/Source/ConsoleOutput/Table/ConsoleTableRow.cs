namespace Me;

public sealed class ConsoleTableRow
{
    private readonly ConsoleTableCell[] _cells;

    public ConsoleTableCell this[int index] => _cells[index];

    public ConsoleTableRow(ConsoleTableCell[] cells, TextAlignmentEnum alignment = TextAlignmentEnum.Left)
    {
        _cells = cells;
    }
}
