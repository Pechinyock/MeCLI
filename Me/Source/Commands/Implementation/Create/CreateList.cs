namespace Me;

internal static class CreateList
{
    public static void Do() 
    {
        var availableToCreate = Create.AvailableSubcommandsWithDescription;
        var rows = new ConsoleTableRow[availableToCreate.Count];
        int index = 0;
        foreach (var item in availableToCreate)
        {
            var rowCells = new ConsoleTableCell[]
            {
                new ConsoleTableCell(item.Key),
                new ConsoleTableCell(item.Value)
            };
            rows[index] = new ConsoleTableRow(rowCells);
            ++index;
        }
        var table = new ConsoleTable(new string[]
                {
                    "Type", "Description"
                }
            , rows
            , new TableDisplaySettings()
            {
                ColumnsWidthPrecents = new int[]
                    {
                        20, 80
                    }
            }
        );
        Print.Table(table);
    }
}
