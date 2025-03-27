namespace Me;

public sealed class ConsoleTableCell
{
    private readonly string _text;
    private readonly TextAlignmentEnum _textAlignment;

    public string Text => _text;
    public TextAlignmentEnum TextAlignment => _textAlignment;

    public ConsoleTableCell(string text
        , TextAlignmentEnum textAlignment = TextAlignmentEnum.Left)
    {
        _text = text ?? String.Empty;
        _textAlignment = textAlignment;
    }
}
