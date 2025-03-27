namespace Me;

public enum TextAlignmentEnum 
{
    Left,
    Center,
    Right,
}

public sealed class TableDisplaySettings
{
    public int[] ColumnsWidthPrecents { get; set; }
    public bool PrintHeader { get; set; } = true;
    public char ColumnSeporator { get; set; } = '|';
    public char RowSeporator { get; set; } = '-';
    public char CrossColumnRowSeporator { get; set; } = '+';
    public char HeaderStroke { get; set; } = '=';
    public char HeaderColumnSeporator { get; set; } = '|';
    public TextAlignmentEnum TitleAlignment { get; set; } = TextAlignmentEnum.Center;
}