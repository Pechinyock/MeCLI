using System.Text;
using System.Diagnostics;

namespace Me;

public sealed class ConsoleTable
{
    public static readonly string LineBreak = Environment.NewLine;

    private const string CUT_OFF_SYMBOLS = "...";
    private const char EMPTY_SYMBOL = ' ';

    private readonly string[] _columnsNames;
    private readonly ConsoleTableRow[] _rows;
    private readonly TableDisplaySettings _displaySettings;
    private readonly int[] _columnsWidth;

    public ConsoleTable(string[] columnsNames
        , ConsoleTableRow[] rows
        , TableDisplaySettings settings = null)
    {
        Debug.Assert(columnsNames is not null);
        Debug.Assert(columnsNames.Length > 0);
        Debug.Assert(rows is not null);

        _columnsNames = columnsNames;
        _rows = rows;

        _displaySettings = settings ?? ApplicationDefaults.GetTableDisplaySettings(_columnsNames.Length);

        _columnsWidth = CalculateColumnsWidth(_displaySettings.ColumnsWidthPrecents);

        Debug.Assert(_columnsWidth.Length == _columnsNames.Length);
    }

    public override string ToString()
    {
        if (_rows is null || _rows.Length == 0) 
            return string.Empty;

        var sb = new StringBuilder();
        if (_displaySettings.PrintHeader)
            AppendHeader(sb);

        AppendRow(sb);

        return sb.ToString();
    }

    private void AppendRow(StringBuilder sb)
    {
        var reservations = new StringBuilder[_columnsNames.Length];

        bool isAppendingReserve = false;

        for (int rowIndex = 0; rowIndex < _rows.Length;)
        {
            var currentRow = _rows[rowIndex];
            if (currentRow is null)
            {
                ++rowIndex;
                continue;
            }

            for (int cellIndex = 0; cellIndex < _columnsNames.Length && !isAppendingReserve; cellIndex++)
            {
                var cell = currentRow[cellIndex] ?? new ConsoleTableCell(null);
                var cellText = cell.Text;
                var allowedLenght = _columnsWidth[cellIndex];
                var cellTextLenght = cellText.Length;
                var cellTextAlignment = cell.TextAlignment;

                if (cellText.Contains(LineBreak))
                {
                    var newLineCharPos = cellText.IndexOf(LineBreak);
                    if (newLineCharPos < allowedLenght - 2)
                    {
                        AppendWithLineBreak(sb
                            , reservations
                            , cellText
                            , newLineCharPos
                            , allowedLenght
                            , cellIndex
                            , cellTextAlignment
                        );
                    }
                    else
                    {
                        AppendCellAndAddReserve(sb
                            , reservations
                            , cellText
                            , allowedLenght
                            , cellIndex
                            , cellTextAlignment
                        );
                    }
                    continue;
                }

                if (cellTextLenght < allowedLenght - 2)
                {
                    AppendCell(sb
                        , cellText
                        , allowedLenght
                        , cellIndex
                        , cellTextAlignment
                    );
                }
                else
                {
                    AppendCellAndAddReserve(sb
                        , reservations
                        , cellText
                        , allowedLenght
                        , cellIndex
                        , cellTextAlignment
                    );
                }
            }

            var hasReserve = false;
            foreach (var reservation in reservations)
            {
                if (reservation is not null)
                {
                    hasReserve = true;
                    break;
                }
            }

            if (hasReserve)
            {
                isAppendingReserve = true;
                for (int reserveIndex = 0; reserveIndex < reservations.Length; ++reserveIndex)
                {
                    var reservation = reservations[reserveIndex];
                    if (reservation is null)
                    {
                        AppendCellWith(sb
                            , reserveIndex
                            , EMPTY_SYMBOL
                            , _displaySettings.ColumnSeporator
                        );
                        continue;
                    }

                    var cellAlignment = currentRow[reserveIndex].TextAlignment;
                    var allowedLenght = _columnsWidth[reserveIndex];
                    var reserveFullText = reservation.ToString();

                    var reserveLenght = reserveFullText.Length;

                    bool itsFit = reserveLenght < allowedLenght;
                    bool hasLineBreak = reserveFullText.Contains(LineBreak);

                    if (itsFit && !hasLineBreak)
                    {
                        AppendCell(sb
                            , reserveFullText
                            , allowedLenght
                            , reserveIndex
                            , cellAlignment
                        );
                        reservations[reserveIndex] = null;
                        continue;
                    }

                    if (hasLineBreak)
                    {
                        var newLineCharPos = reserveFullText.IndexOf(LineBreak);
                        if (newLineCharPos < allowedLenght - 2)
                        {
                            AppendWithLineBreak(sb
                                , reservations
                                , reserveFullText
                                , newLineCharPos
                                , allowedLenght
                                , reserveIndex
                                , cellAlignment
                            );
                        }
                        else
                        {
                            AppendCellAndAddReserve(sb
                                , reservations
                                , reserveFullText
                                , allowedLenght
                                , reserveIndex
                                , cellAlignment
                            );
                        }
                        continue;
                    }

                    AppendCellAndAddReserve(sb
                        , reservations
                        , reserveFullText
                        , allowedLenght
                        , reserveIndex
                        , cellAlignment
                    );
                }

                continue;
            }

            isAppendingReserve = false;
            for (int cellIndex = 0; cellIndex < _columnsNames.Length; ++cellIndex)
            {
                AppendCellWith(sb
                    , cellIndex
                    , _displaySettings.RowSeporator
                    , _displaySettings.CrossColumnRowSeporator
                );
            }

            ++rowIndex;
        }
    }

    private void AppendWithLineBreak(StringBuilder sb
        , StringBuilder[] reservers
        , string text
        , int lineBreakIndex
        , int allowedLenght
        , int columnIndex
        , TextAlignmentEnum textAlignment)
    {
        var textToAppend = text.Substring(0, lineBreakIndex);
        AppendCell(sb
            , textToAppend
            , allowedLenght
            , columnIndex
            , textAlignment
        );

        var newLineCharLenght = LineBreak.Length;

        var textWithNewLine = text.Substring(lineBreakIndex + newLineCharLenght
            , text.Length - lineBreakIndex - newLineCharLenght
        );
        AddReserve(reservers, columnIndex, textWithNewLine);
    }

    private void AppendCellAndAddReserve(StringBuilder sb
        , StringBuilder[] reserves
        , string text
        , int allowedLenght
        , int columnIndex
        , TextAlignmentEnum cellAlignment)
    {
        var splitPoint = GetSplitPoint(text, allowedLenght);
        var fittingPart = text.Substring(0, splitPoint);
        AppendCell(sb
            , fittingPart
            , allowedLenght
            , columnIndex
            , cellAlignment
        );
        var textToPrintLater = text.Substring(splitPoint, text.Length - splitPoint);
        AddReserve(reserves, columnIndex, textToPrintLater);
    }

    private int GetSplitPoint(string fullText, int allowedLenght)
    {
        var outIndex = allowedLenght - 4;
        while (outIndex != 0)
        {
            var currentCharacter = fullText[outIndex];
            if (currentCharacter == EMPTY_SYMBOL)
            {
                return outIndex;
            }
            --outIndex;
        }
        return allowedLenght - 3;
    }

    private static void AddReserve(StringBuilder[] allReservers, int index, string textToStore)
    {
        var newReserve = new StringBuilder();
        var savingText = textToStore.StartsWith(EMPTY_SYMBOL)
            ? textToStore.Substring(1, textToStore.Length - 1)
            : textToStore;

        newReserve.Append(savingText);
        allReservers[index] = newReserve;
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
            AppendCellWith(sb
                , i
                , _displaySettings.HeaderStroke
                , _displaySettings.HeaderStroke
            );
        }
        for (int i = 0; i < _columnsNames.Length; ++i)
        {
            var columnWidth = _columnsWidth[i];

            var title = columnWidth >= _columnsNames[i].Length
                ? _columnsNames[i]
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
            AppendCellWith(sb
                , i
                , _displaySettings.HeaderStroke
                , _displaySettings.HeaderStroke
            );
        }
    }

    private void AppendLeftCell(StringBuilder sb
        , string text
        , int columnWidth
        , int index)
    {
        var charDiff = columnWidth - text.Length - 1;
        sb.Append(EMPTY_SYMBOL);
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
        , string text
        , int columnWidth
        , int index)
    {
        var charDiff = columnWidth - text.Length;
        while (charDiff > 0)
        {
            if (charDiff == 2)
                break;

            sb.Append(EMPTY_SYMBOL);
            --charDiff;
        }

        sb.Append(text);
        sb.Append(EMPTY_SYMBOL);
        if (!IsLastCell(index))
            sb.Append(_displaySettings.ColumnSeporator);
        else
            sb.Append(Environment.NewLine);
    }

    private string CutTitle(int index)
    {
        Debug.Assert(index >= 0 && index < _columnsNames.Length);
        var title = _columnsNames[index];
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
