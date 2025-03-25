namespace Me;

public enum TitleAlignmentEnum 
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
    public TitleAlignmentEnum TitleAlignment { get; set; } = TitleAlignmentEnum.Center;
}