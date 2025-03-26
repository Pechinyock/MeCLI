namespace Me;

public sealed class ConsoleTableCell
{
    private readonly string _text;
    private readonly TextAlignmentEnum _textAlignment;
    private readonly int _columnIndex;

    public string Text => _text;
    public TextAlignmentEnum TextAlignment => _textAlignment;

    public ConsoleTableCell(int columnIndex
        , string text
        , TextAlignmentEnum textAlignment = TextAlignmentEnum.Left)
    {
        _text = text ?? String.Empty;
        _textAlignment = textAlignment;
        _columnIndex = columnIndex;
    }
}
