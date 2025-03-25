using System.Diagnostics;
using System.Text;

namespace Me;

public sealed class ConsoleTable
{
    private const string CUT_OFF_SYMBOLS = "...";
    private const char EMTY_SYMBOL = ' ';

    private readonly string[] _columns;
    private readonly TableRowData[] _rows;
    private readonly TableDisplaySettings _displaySettings;
    private readonly int[] _columnsWidth;

    public ConsoleTable(string[] columns
        , TableRowData[] rows
        , TableDisplaySettings settings = null)
    {
        Debug.Assert(columns is not null);
        Debug.Assert(columns.Length > 0);
        Debug.Assert(rows is not null);

        _columns = columns;
        _rows = rows;

        _displaySettings = settings ?? ApplicationDefaults.GetTableDisplaySettings(_columns.Length);

        _columnsWidth = CalculateColumnsWidth(_displaySettings.ColumnsWidthPrecents);

        Debug.Assert(_columnsWidth.Length == _columns.Length);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        if (_displaySettings.PrintHeader)
            AppendHeader(sb);

        AppendRow(sb);

        return sb.ToString();
    }

    private void AppendRow(StringBuilder sb) 
    {
        /* damn this is not so easy as I thought... 
         * first thing first not enought clarity...
         * get full row, define cells are filled and
         * the fill row as it needs to be printed.
         * if row.Index != i - it is empty row.
         * and it is only for that case when cell have enough space for text...
         */
        for (int i = 0; i < _rows.Length; ++i) 
        {
            var columnIndex = _rows[i].Index;
            Debug.Assert(columnIndex >= 0 && columnIndex < _columns.Length);

            var allowedLenght = _columnsWidth[columnIndex];
            var cellText = _rows[i].Data;
            var cellTextLenght = cellText.Length;
            if (allowedLenght >= cellTextLenght) 
            {

                sb.Append(cellText);

                var spinToCellEnd = allowedLenght - cellText.Length;
                while (spinToCellEnd > 1) 
                {
                    sb.Append(EMTY_SYMBOL);
                    --spinToCellEnd;
                }
                if(i != _columns.Length - 1)
                    sb.Append(_displaySettings.ColumnSeporator);
            }

            var isRowEnded = i == _columns.Length - 1;
            if (isRowEnded)
            {
                sb.Append(Environment.NewLine);
                for (int j = 0; j < _columns.Length; ++j)
                    AppendLine(sb, j, _displaySettings.RowSeporator);
            }
        }
    }

    private void AppendHeader(StringBuilder sb)
    {
        for (int i = 0; i < _columnsWidth.Length; ++i)
        {
            AppendLine(sb, i, '=');
        }
        for (int i = 0; i < _columns.Length; ++i)
        {
            var columnWidth = _columnsWidth[i];

            var title = columnWidth >= _columns[i].Length
                ? _columns[i]
                : CutTitle(i);

            switch (_displaySettings.TitleAlignment) 
            {
                case TitleAlignmentEnum.Left:
                    AppendLeftColumnTitle(sb, title, columnWidth, i);
                    break;
                case TitleAlignmentEnum.Center:
                    AppendCenterColumnTitle(sb, title, columnWidth, i);
                    break;
                case TitleAlignmentEnum.Right:
                    AppendRightColumnTitle(sb, title, columnWidth, i);
                    break;
            }
        }
        for (int i = 0; i < _columnsWidth.Length; ++i)
        {
            AppendLine(sb, i, '=');
        }
    }

    private void AppendLeftColumnTitle(StringBuilder sb
        , string title
        , int columnWidth
        , int index) 
    {
        var charDiff = columnWidth - title.Length;
        sb.Append(title);
        while (charDiff > 0)
        {
            if (charDiff == 2)
            {
                sb.Append(EMTY_SYMBOL);
                if (index != _columns.Length - 1)
                    sb.Append(_displaySettings.ColumnSeporator);
                else
                    sb.Append(Environment.NewLine);
                break;
            }
            sb.Append(EMTY_SYMBOL);
            --charDiff;
        }
    }

    private void AppendCenterColumnTitle(StringBuilder sb
        , string title
        , int columnWidth
        , int index)
    {
        var charDiff = columnWidth - title.Length;
        int leftSpan = charDiff - (charDiff / 2);
        int rightSpan = charDiff -leftSpan;

        Debug.Assert((leftSpan + rightSpan) == charDiff);

        while (leftSpan > 0) 
        {
            sb.Append(EMTY_SYMBOL);
            --leftSpan;
        }
        sb.Append(title);
        while (rightSpan > 0)
        {
            if (rightSpan == 2)
            {
                sb.Append(EMTY_SYMBOL);
                if (index != _columns.Length - 1)
                    sb.Append(_displaySettings.ColumnSeporator);
                else
                    sb.Append(Environment.NewLine);
                break;
            }
            sb.Append(EMTY_SYMBOL);
            --rightSpan;
        }
    }

    private void AppendRightColumnTitle(StringBuilder sb
        , string title
        , int columnWidth
        , int index)
    {
        var charDiff = columnWidth - title.Length;
        while (charDiff > 0)
        {
            if (charDiff == 2)
                break;

            sb.Append(EMTY_SYMBOL);
            --charDiff;
        }

        sb.Append(title);
        sb.Append(EMTY_SYMBOL);
        if (index != _columns.Length - 1)
            sb.Append(_displaySettings.ColumnSeporator);
        else
            sb.Append(Environment.NewLine);
    }

    private string CutTitle(int index)
    {
        Debug.Assert(index >= 0 && index < _columns.Length);
        var title =  _columns[index];
        /* will cut hardly (coz it is just for debugging) */
        var allowedWidth = _columnsWidth[index] - (CUT_OFF_SYMBOLS.Length + 5);
        var actualWidth = title.Length;
        var sb = new StringBuilder(title.Substring(0, allowedWidth));
        sb.Append(CUT_OFF_SYMBOLS);
        return sb.ToString();
    }

    private void AppendLine(StringBuilder sb, int columnIndex, char caracter)
    {
        Debug.Assert(sb is not null);
        Debug.Assert(columnIndex >= 0 && columnIndex <= _columnsWidth.Length);

        var cellWidth = _columnsWidth[columnIndex];
        bool isLastCharacterSlot = columnIndex == _columnsWidth.Length - 1;
        while (cellWidth > 0)
        {
            if (isLastCharacterSlot && cellWidth == 1)
            {
                sb.Append(Environment.NewLine);
                break;
            }
            sb.Append(caracter);
            --cellWidth;
        }
    }

    private int[] CalculateColumnsWidth(params int[] precents)
    {
        var result = new int[precents.Length];
        for (int i = 0; i < precents.Length; ++i)
        {
            result[i] = CalculateColumnwidth(precents[i]);
        }
        return result;
    }

    private static int CalculateColumnwidth(int precents)
    {
        Debug.Assert(precents > 0 && precents <= 100);

        var consoleWidth = Console.WindowWidth;

        int widthOut = (consoleWidth * precents) / 100;
        return widthOut;
    }
}
