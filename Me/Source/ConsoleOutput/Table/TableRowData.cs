using System.Diagnostics;

namespace Me;

public sealed class TableRowData
{
    private readonly int _columnIndex;
    private readonly string _data;

    public int Index => _columnIndex;
    public string Data => _data;

    public TableRowData(int columnIndex, string data)
    {
        Debug.Assert(columnIndex >= 0);
        _columnIndex = columnIndex;
        _data = data;
    }
}
