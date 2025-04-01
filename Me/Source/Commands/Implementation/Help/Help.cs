using System.Text;
using System.Diagnostics;

namespace Me;

internal sealed class Help : MeCommandBase
                           , ISubcommanded
                           , IDescribed
{
    private string[] _passedSubcommands;

    private readonly Dictionary<string, string> _subcommandsWithDescription = new()
    {
        { "<any command>", "prints specific command(s) help" },
        { "<any character>", "prits help for command(s) that starts with passed character(s)"}
    };

    public override string Alias => "help";
    public override bool Validate() 
    {
        return true;
    }

    public override CmdTypeEnumFlag GetTypes => CmdTypeEnumFlag.Described | CmdTypeEnumFlag.Subcommanded;
    public string GetDescription() => "Prints information about all available commands";

    public void SetSubcommand(string[] subCommands) => _passedSubcommands = subCommands;
    public Dictionary<string, string> GetAvailableSubcommands() => _subcommandsWithDescription;

    public override void Execute()
    {
        if (_passedSubcommands is null || _passedSubcommands.Length == 0)
            PrintFullHelp();
        else
            PrintWitSubcommands();
    }

    private void PrintWitSubcommands() 
    {
        var subCommands = _passedSubcommands;
        var addedDict = new Dictionary<string, MeCommandBase>();

        foreach (var sbCmd in subCommands) 
        {
            if (sbCmd.Length == 1)
            {
                var singleCharWord = sbCmd;

                var mentToadd = Librarian.Request(sbCmd[0]);
                if (mentToadd is null)
                {
                    Print.Error($"Couldn't find commands that starts with letter '{sbCmd}'");
                    continue;
                }

                foreach (var founded in mentToadd)
                {
                    if (addedDict.ContainsKey(founded.Alias))
                        continue;

                    addedDict.Add(founded.Alias, founded);
                }
            }

            if (sbCmd.Length > 1)
            {
                if (addedDict.ContainsKey(sbCmd))
                    continue;

                var mentToAdd = Librarian.Request(sbCmd);
                if (mentToAdd is null)
                {
                    Print.Error($"Couldn't find command:{sbCmd}");
                    continue;
                }

                if(!addedDict.ContainsKey(mentToAdd.Alias))
                    addedDict.Add(mentToAdd.Alias, mentToAdd);
            }
        }

        var cmdArray = addedDict.Values.ToArray();
        var table = ConstrucTable(cmdArray);
        Print.Table(table);
    }

    private void PrintFullHelp() 
    {
        var commads = Librarian.RequestAll();
        var table = ConstrucTable(commads);
        Print.Table(table);
    }

    private static ConsoleTable ConstrucTable(MeCommandBase[] cmds)
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

        var rows = new ConsoleTableRow[cmds.Length];

        for (int rowIndex = 0; rowIndex < rows.Length; ++rowIndex)
        {
            var cells = new ConsoleTableCell[3];
            var cmd = cmds[rowIndex];
            cells[0] = new ConsoleTableCell(cmds[rowIndex].Alias);

            var argsAndParams = ConcatAllInfo(cmd);
            cells[1] = new ConsoleTableCell(argsAndParams);

            if (cmd.IsDescribed())
            {
                var described = cmd as IDescribed;
                cells[2] = new ConsoleTableCell(described.GetDescription(), TextAlignmentEnum.Center);
            }

            rows[rowIndex] = new ConsoleTableRow(cells);
        }
        var result = new ConsoleTable(columns, rows, displaySettings);
        return result;
    }

    private static string ConcatAllInfo(MeCommandBase cmd)
    {
        Debug.Assert(cmd.IsParametrized() || cmd.IsArgumented() || cmd.IsSubcommanded());

        var sb = new StringBuilder();
        if (cmd.IsParametrized())
        {
            var parametrized = cmd as IParametrized;
            var parameters = parametrized.GetAvailableParameters();
            var paramIndicator = parametrized.GetParameterIndicator();
            AppendDictionary(sb, parameters, paramIndicator);
            sb.Append(ConsoleTable.LineBreak);
        }
        
        if (cmd.IsArgumented())
        {
            var argumented = cmd as IArgumented;
            var arguments = argumented.GetAvailableArgs();
            var argsIndicator = argumented.GetArgumentIndicator();
            AppendDictionary(sb, arguments, argsIndicator);
            sb.Append(ConsoleTable.LineBreak);
        }

        if (cmd.IsSubcommanded())
        {
            var subcommanded = cmd as ISubcommanded;
            var subcommands = subcommanded.GetAvailableSubcommands();
            AppendDictionary(sb, subcommands, "");
            sb.Append(ConsoleTable.LineBreak);
        }

        return sb.ToString();
    }

    private static void AppendDictionary(StringBuilder sb, Dictionary<string, string> dict, string indicatior)
    {
        if (dict is null || dict.Count == 0)
            return;

        int index = 0;
        foreach (var kvp in dict)
        {
            sb.Append($"{indicatior}{kvp.Key} - {kvp.Value}{ConsoleTable.LineBreak}");
            index++;
        }
    }
}
