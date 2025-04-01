namespace Me;

internal static class CreateList
{
    private static readonly Dictionary<string, string> _availableToCreate = new()
    {
        { /* project */  Create.AvailableTypesToCreate[0], "creates new project" },
    };

    public static void Do() 
    {
        var rows = new ConsoleTableRow[_availableToCreate.Count];
        int index = 0;
        foreach (var item in _availableToCreate) 
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
