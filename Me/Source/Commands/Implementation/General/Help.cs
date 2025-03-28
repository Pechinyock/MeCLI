using System.Text;
using System.Diagnostics;

namespace Me;

internal sealed class Help : MeCommandBase
                           , IArgumented
                           , IDescribed
{
    private string[] _passedArguments;

    private readonly Dictionary<string, string> _argsWithDescription = new()
    {
        { "<command>", "prints specific command help " }
    };

    public override string Alias => "help";
    public override CmdTypeEnumFlag GetTypes => CmdTypeEnumFlag.Argumented | CmdTypeEnumFlag.Described;
    public string GetArgumentIndicator() => "";
    public void SetArguments(string[] value) => _passedArguments = value;
    public string[] GetPassedArguments() => _passedArguments;
    public Dictionary<string, string> GetAvailableArgs() => _argsWithDescription;
    public string[] GetArgsWithdescription() => CommandsDefaults.GetArgsOrParamsDescriptionDefault(_argsWithDescription, GetArgumentIndicator());
    public string GetDescription() => "Prints information about all available commands";

    public override void Execute()
    {
        var columns = new string[]
        {
            "Alias",
            "Args\\params",
            "Description",
        };

        var displaySettings = new TableDisplaySettings()
        {
            ColumnsWidthPrecents = new[] { 15, 40, 45 }
        };
        if (_passedArguments is null || _passedArguments.Length == 0)
        {
            /* Print full help */
            var commads = Librarian.RequestAll();
            var rows = new ConsoleTableRow[commads.Length];

            for (int rowIndex = 0; rowIndex < rows.Length; ++rowIndex)
            {
                var cells = new ConsoleTableCell[columns.Length];
                var cmd = commads[rowIndex];
                cells[0] = new ConsoleTableCell(commads[rowIndex].Alias);

                if (cmd.IsParametrized() || cmd.IsArgumented())
                {
                    var parametrized = cmd as IParametrized;
                    var argsAndParams = ConcatAllParamsAndArgs(cmd);
                    cells[1] = new ConsoleTableCell(argsAndParams);
                }

                if (cmd.IsDescribed()) 
                {
                    var described = cmd as IDescribed;
                    cells[2] = new ConsoleTableCell(described.GetDescription(), TextAlignmentEnum.Center);
                }

                rows[rowIndex] = new ConsoleTableRow(cells);
            }
            Print.Table(new ConsoleTable(columns, rows, displaySettings));
            return;
        }

        //if (_passedArguments.Length == 1 && _passedArguments[0].Length == 1)
        //{
        //    /* Print print all cmds that starts with character */
        //    return;
        //}
        //else 
        //{
        //    /* print help single coomand */
        //}
    }

    private static string ConcatAllParamsAndArgs(MeCommandBase cmd) 
    {
        Debug.Assert(cmd.IsParametrized() || cmd.IsArgumented());

        var parametrized = cmd as IParametrized;
        var sb = new StringBuilder();
        if (parametrized is not null) 
        {
            var parameters = parametrized.GetAvailableParameters();
            var paramIndicator = parametrized.GetParameterIndicator();

            foreach (var p in parameters)
            {
                sb.Append($"{paramIndicator}{p.Key} - {p.Value}{ConsoleTable.LineBreak}");
            }
        }

        var argumented = cmd as IArgumented;
        if (argumented is not null) 
        {
            var arguments = argumented.GetAvailableArgs();
            var argsIndicator = argumented.GetArgumentIndicator();

            foreach (var a in arguments)
            {
                sb.Append($"{argsIndicator}{a.Key} - {a.Value}{ConsoleTable.LineBreak}");
            }
        }

        return sb.ToString();
    }
}
