using System.Text;
using System.Diagnostics;

namespace Me;

public sealed class ConsoleTable
{
    private const string CUT_OFF_SYMBOLS = "...";
    private const char EMPTY_SYMBOL = ' ';

    private readonly string[] _columns;
    private readonly ConsoleTableRow[] _rows;
    private readonly TableDisplaySettings _displaySettings;
    private readonly int[] _columnsWidth;

    public ConsoleTable(string[] columns
        , ConsoleTableRow[] rows
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
        var reservations = new StringBuilder[_columns.Length];
        foreach (var row in _rows)
        {
            for (int i = 0; i < _columns.Length; i++)
            {
                var cellText = row[i].Text;
                var allowedLenght = _columnsWidth[i];
                var cellTextLenght = cellText.Length;
                var cellTextAlignment = row[i].TextAlignment;

                if (cellTextLenght < allowedLenght - 2)
                {
                    AppendCell(sb, cellText, allowedLenght, i, cellTextAlignment);
                }
                else
                {
                    var splitPoint = allowedLenght - 3;
                    var fitsPart = cellText.Substring(0, splitPoint);
                    AppendCell(sb, fitsPart, allowedLenght, i, cellTextAlignment);
                    var reservation = new StringBuilder();
                    reservation.Append(cellText.Substring(splitPoint, cellText.Length - splitPoint));
                    reservations[i] = reservation;
                }
            }

            for (int j = 0; j < _columns.Length; ++j)
                AppendCellWith(sb, j, _displaySettings.RowSeporator, '+');
        }
    }

    private void AppendCell(StringBuilder sb
        , string text
        , int columnWidth
        , int columnIndex
        , TextAlignmentEnum alignment)
    {
        switch (alignment)
        {
            case TextAlignmentEnum.Left:
                AppendLeftCell(sb, text, columnWidth, columnIndex);
                break;
            case TextAlignmentEnum.Center:
                AppendCenterCell(sb, text, columnWidth, columnIndex);
                break;
            case TextAlignmentEnum.Right:
                AppendRightCell(sb, text, columnWidth, columnIndex);
                break;
        }
    }

    private void AppendHeader(StringBuilder sb)
    {
        for (int i = 0; i < _columnsWidth.Length; ++i)
        {
            AppendCellWith(sb, i, '=', '=');
        }
        for (int i = 0; i < _columns.Length; ++i)
        {
            var columnWidth = _columnsWidth[i];

            var title = columnWidth >= _columns[i].Length
                ? _columns[i]
                : CutTitle(i);

            switch (_displaySettings.TitleAlignment)
            {
                case TextAlignmentEnum.Left:
                    AppendLeftCell(sb, title, columnWidth, i);
                    break;
                case TextAlignmentEnum.Center:
                    AppendCenterCell(sb, title, columnWidth, i);
                    break;
                case TextAlignmentEnum.Right:
                    AppendRightCell(sb, title, columnWidth, i);
                    break;
            }
        }
        for (int i = 0; i < _columnsWidth.Length; ++i)
        {
            AppendCellWith(sb, i, '=', '=');
        }
    }

    private void AppendLeftCell(StringBuilder sb
        , string text
        , int columnWidth
        , int index)
    {
        var charDiff = columnWidth - text.Length;
        sb.Append(text);
        while (charDiff > 0)
        {
            if (charDiff == 2)
            {
                sb.Append(EMPTY_SYMBOL);
                if (!IsLastCell(index))
                    sb.Append(_displaySettings.ColumnSeporator);
                else
                    sb.Append(Environment.NewLine);
                break;
            }
            sb.Append(EMPTY_SYMBOL);
            --charDiff;
        }
    }

    private void AppendCenterCell(StringBuilder sb
        , string text
        , int columnWidth
        , int index)
    {
        var charDiff = columnWidth - text.Length;
        int rightSpan = charDiff - (charDiff / 2);
        int leftSpan = charDiff - rightSpan;

        Debug.Assert((leftSpan + rightSpan) == charDiff);

        while (leftSpan > 0)
        {
            sb.Append(EMPTY_SYMBOL);
            --leftSpan;
        }
        sb.Append(text);
        while (rightSpan > 0)
        {
            if (rightSpan == 2)
            {
                sb.Append(EMPTY_SYMBOL);
                if (!IsLastCell(index))
                    sb.Append(_displaySettings.ColumnSeporator);
                else
                    sb.Append(Environment.NewLine);
                break;
            }
            sb.Append(EMPTY_SYMBOL);
            --rightSpan;
        }
    }

    private void AppendRightCell(StringBuilder sb
        , string title
        , int columnWidth
        , int index)
    {
        var charDiff = columnWidth - title.Length;
        while (charDiff > 0)
        {
            if (charDiff == 2)
                break;

            sb.Append(EMPTY_SYMBOL);
            --charDiff;
        }

        sb.Append(title);
        sb.Append(EMPTY_SYMBOL);
        if (!IsLastCell(index))
            sb.Append(_displaySettings.ColumnSeporator);
        else
            sb.Append(Environment.NewLine);
    }

    private string CutTitle(int index)
    {
        Debug.Assert(index >= 0 && index < _columns.Length);
        var title = _columns[index];
        /* will cut hardly (coz it is just for debugging) */
        var allowedWidth = _columnsWidth[index] - (CUT_OFF_SYMBOLS.Length + 5);
        var actualWidth = title.Length;
        var sb = new StringBuilder(title.Substring(0, allowedWidth));
        sb.Append(CUT_OFF_SYMBOLS);
        return sb.ToString();
    }

    private void AppendCellWith(StringBuilder sb
        , int columnIndex
        , char character
        , char columnSeporator)
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
            var toPrint = cellWidth != 1
                ? character
                : columnSeporator;

            sb.Append(toPrint);

            --cellWidth;
        }
    }

    private bool IsLastCell(int index)
    {
        return (index + 1) % _columnsWidth.Length == 0;
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
