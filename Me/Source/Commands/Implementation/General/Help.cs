using System.Text;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations;

namespace Me;

internal sealed class Help : MeCommandBase
                           , IArgumented
                           , IDescribed
{
    #region Nested
    private enum ColumnsEnum
    {
        Alias = 0,
        ParamSlashArgs = 1,
        Description = 2
    }

    private class CommandInfo
    {
        public string Alias { get; private set; }
        public string[] ArgsAndParams { get; private set; }
        public string Description { get; private set; }

        public CommandInfo(string alias, string[] args, string[] parameters, string description)
        {
            var argsCount = args == null ? 0 : args.Length;
            var paramsCount = parameters == null ? 0 : parameters.Length;
            var argsAndParamsCount = argsCount + paramsCount;
            var appendArgsAndParams = new string[argsAndParamsCount];
            int index = 0;
            if (argsCount > 0)
            {
                foreach (var a in args)
                {
                    appendArgsAndParams[index] = a;
                    ++index;
                }
            }
            if (paramsCount > 0)
            {
                foreach (var p in parameters)
                {
                    appendArgsAndParams[index] = p;
                    ++index;
                }
            }
            ArgsAndParams = appendArgsAndParams;
            Alias = alias;
            Description = description;
        }

        public string GetFormated() 
        {
            var sb = new StringBuilder();

            var aliasColumWidth = TableSpec.CalculateColumnwidth(ColumnsEnum.Alias, Console.WindowWidth);
            var argsParamsColumWidth = TableSpec.CalculateColumnwidth(ColumnsEnum.ParamSlashArgs, Console.WindowWidth);
            var descriptioonColumWidth = TableSpec.CalculateColumnwidth(ColumnsEnum.Description, Console.WindowWidth);

            var aliasRowsCount = 1;
            var paramsAndArgs = FitArgsParamsToCell();
            var argsParamsRowsCount = paramsAndArgs is null
                                    ? 0
                                    : paramsAndArgs.Length;

            var formatedDescripton = FitDescriptionToCell();
            var descriptioonRowsCount = formatedDescripton is null
                                      ? 0
                                      : formatedDescripton.Length;

            var totalRows = Math.Max(argsParamsRowsCount, descriptioonRowsCount);


            for (int i = 0; i < totalRows; ++i) 
            {
                if (i < aliasRowsCount)
                {
                    AppendToCellEnd(sb, Alias, aliasColumWidth);
                }
                else 
                {
                    AppendEmptyCell(sb, aliasColumWidth);
                }

                if (i < argsParamsRowsCount)
                {
                    AppendToCellEnd(sb, paramsAndArgs[i], argsParamsColumWidth);
                }
                else 
                {
                    AppendEmptyCell(sb, argsParamsColumWidth);
                }

                if (i < descriptioonRowsCount)
                {
                    AppendToCellEnd(sb, formatedDescripton[i], descriptioonColumWidth);
                    sb.Append($"{Environment.NewLine}");
                }
                else 
                {
                    AppendEmptyCell(sb, descriptioonColumWidth);
                    sb.Append($"{Environment.NewLine}");
                }
            }
            return sb.ToString();
        }

        private void AppendToCellEnd(StringBuilder sb, string value, int columnWidth)
        {
            Debug.Assert(sb is not null);
            Debug.Assert(value is not null);
            Debug.Assert(value.Length <= columnWidth);

            sb.Append(value);

            int span = columnWidth - value.Length;
            while (span > 0) 
            {
                sb.Append(" ");
                --span;
            }
        }

        private void AppendEmptyCell(StringBuilder sb, int columntWidth) 
        {
            Debug.Assert(sb is not null);

            while (columntWidth > 0) 
            {
                sb.Append(" ");
                --columntWidth;
            }
        }

        private string[] FitArgsParamsToCell() 
        {
            var fitToWidth = TableSpec.CalculateColumnwidth(ColumnsEnum.ParamSlashArgs, Console.WindowWidth);
            var rowSplitedText = new List<string>();
            foreach (var element in ArgsAndParams) 
            {
                var words = element.Split(' ');
                var freeSpace = fitToWidth;
                var sb = new StringBuilder();
                foreach (var word in words) 
                {
                    if (String.IsNullOrEmpty(word))
                        continue;

                    var wordCharCount = word.Length;
                    if (wordCharCount > freeSpace) 
                    {
                        rowSplitedText.Add(sb.ToString());
                        sb.Clear();
                        freeSpace = fitToWidth;
                        sb.Append($"{word} ");
                        freeSpace -= wordCharCount + 1;
                        continue;
                    }
                    sb.Append($"{word} ");
                    freeSpace -= wordCharCount + 1;
                }
                rowSplitedText.Add(sb.ToString());
            }
            return rowSplitedText.ToArray();
        }

        private string[] FitDescriptionToCell()
        {
            if (String.IsNullOrEmpty(Description))
                return null;

            var fitToWidth = TableSpec.CalculateColumnwidth(ColumnsEnum.Description, Console.WindowWidth);
            var rowSlplitedText = new List<string>();
            var words = Description.Split(' ');
            var sb = new StringBuilder();
            var freeSpace = fitToWidth;
            foreach (var word in words)
            {
                var wordCharCount = word.Length;
                if (wordCharCount > freeSpace)
                {
                    rowSlplitedText.Add(sb.ToString());
                    sb.Clear();
                    freeSpace = fitToWidth;
                    sb.Append($"{word} ");
                    /* +1 it is space after word */
                    freeSpace -= wordCharCount + 1;
                    continue;
                }
                sb.Append($"{word} ");
                /* +1 it is space after word */
                freeSpace -= wordCharCount + 1;
            }
            rowSlplitedText.Add(sb.ToString());
            return rowSlplitedText.ToArray();
        }
    }

    private static class TableSpec
    {
        public static readonly Dictionary<ColumnsEnum, int> ColumnsSizePresentage = new()
        {
            { ColumnsEnum.Alias, 10 },
            { ColumnsEnum.ParamSlashArgs, 30 },
            { ColumnsEnum.Description, 60 }
        };

        public static int CalculateColumnwidth(ColumnsEnum column, int width)
        {
            Debug.Assert(ColumnsSizePresentage.ContainsKey(column));
            var precents = ColumnsSizePresentage[column];

            int widthOut = (width * precents) / 100;
            return widthOut;
        }
    }

    #endregion

    private string[] _passedArguments;

    private readonly Dictionary<string, string> _argsWithDescription = new()
    {
        { "<command>", "prints specific command help " }
    };

    public override string Alias => "help";

    public override CmdTypeEnumFlag GetTypes => CmdTypeEnumFlag.Argumented | CmdTypeEnumFlag.Described;

    public override void Execute()
    {
        if (_passedArguments is null || _passedArguments.Length == 0)
        {
            PrintGlobalHelp();
            return;
        }
        if (_passedArguments.Length == 1 && _passedArguments[0].Length == 1)
        {
            PrintLetterHelp(_passedArguments[0][0]);
            return;
        }
        PrintCommandHelp();
    }

    public string GetArgumentIndicator() => "";

    public void SetArguments(string[] value) => _passedArguments = value;

    public string[] GetPassedArguments() => _passedArguments;

    public Dictionary<string, string> GetAvailableArgs() => _argsWithDescription;

    public string[] GetArgsWithdescription() => CommandsDefaults.GetArgsOrParamsDescriptionDefault(_argsWithDescription, GetArgumentIndicator());

    public string GetDescription() => "Prints information about all available commands";

    private void PrintCommandHelp()
    {
        var toPrint = new List<MeCommandBase>(_passedArguments.Length);
        foreach (var passed in _passedArguments) 
        {
            var cmd = Librarian.Request(passed);
            if (cmd == null)
            {
                Log.Error($"Command: {passed} not found");
                continue;
            }
            toPrint.Add(cmd);
        }
        PrintCommandHelp(toPrint.ToArray());
    }

    private void PrintLetterHelp(char letter)
    {
        var requestedCommands = Librarian.Request(letter);
        if (requestedCommands is null || requestedCommands.Length == 0)
        {
            Log.Error($"Commands thas starts with character '{letter}' not found");
            return;
        }
        PrintCommandHelp(requestedCommands);
    }

    private void PrintGlobalHelp()
    {
        var allAvailableCommands = Librarian.RequestAll();
        PrintCommandHelp(allAvailableCommands);
    }

    private void PrintCommandHelp(MeCommandBase[] cmds)
    {
        foreach (var cmd in cmds)
        {
            var info = CollectInforamation(cmd);
            var printIt = info.GetFormated();
            Print.String(printIt);
        }
    }

    private CommandInfo CollectInforamation(MeCommandBase cmd)
    {
        Debug.Assert(cmd is not null);
        int currentConsoleWidth = Console.WindowWidth;

        string commandAlias = cmd.Alias;

        string[] commandArgs = null;
        if (cmd.IsArgumented())
        {
            var argumented = cmd as IArgumented;
            commandArgs = argumented.GetArgsWithdescription();
        }

        string[] commandParams = null;
        if (cmd.IsParametrized())
        {
            var parametrized = cmd as IParametrized;
            commandParams = parametrized.GetParamsWithDescription();
        }

        string commandDescription = null;
        if (cmd.IsDescribed())
        {
            var described = cmd as IDescribed;
            commandDescription = described.GetDescription();
        }
        var commandInfo = new CommandInfo(commandAlias, commandArgs, commandParams, commandDescription);

        return commandInfo;
    }
}
