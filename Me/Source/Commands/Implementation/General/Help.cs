using System.Text;
using System.Diagnostics;

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
        public string[] ParamsAndArgs { get; private set; }
        public string[] Description { get; private set; }

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
            ParamsAndArgs = appendArgsAndParams;
            Alias = alias;
            Description = SplitDescriptionByRows(description);
        }

        public int GetTotalRows()
        {
            var totalParamsArgsRows = GetArgsParamsRowsCount();
            var totalDescriptionRows = GetDescriptionRowsCount();

            var totalRows = Math.Max(totalParamsArgsRows, totalDescriptionRows);

            return totalRows;
        }

        public int GetArgsParamsRowsCount()
        {
            return ParamsAndArgs is null
                   ? 0
                   : ParamsAndArgs.Length;
        }

        public int GetDescriptionRowsCount() 
        {
            return Description is null
                   ? 0
                   : Description.Length;
        }

        public string GetFormated() 
        {
            var sb = new StringBuilder();

            var aliasColumWidth = TableSpec.CalculateColumnwidth(ColumnsEnum.Alias, Console.WindowWidth);
            var argsParamsColumWidth = TableSpec.CalculateColumnwidth(ColumnsEnum.ParamSlashArgs, Console.WindowWidth);
            var descriptioonColumWidth = TableSpec.CalculateColumnwidth(ColumnsEnum.Description, Console.WindowWidth);

            var aliasRowsCount = 1;
            var argsParamsRowsCount = GetArgsParamsRowsCount();
            var descriptioonRowsCount = GetDescriptionRowsCount();

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
                    AppendToCellEnd(sb, ParamsAndArgs[i], argsParamsColumWidth);
                }
                else 
                {
                    AppendEmptyCell(sb, argsParamsColumWidth);
                }

                if (i < descriptioonRowsCount)
                {
                    AppendToCellEnd(sb, Description[i], descriptioonColumWidth);
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

        private string[] SplitDescriptionByRows(string sourceText)
        {
            if (String.IsNullOrEmpty(sourceText))
                return null;

            var fitToWidth = TableSpec.CalculateColumnwidth(ColumnsEnum.Description, Console.WindowWidth);
            var rowSlplitedText = new List<string>();
            var words = sourceText.Split(' ');
            var sb = new StringBuilder();
            var freeSpace = fitToWidth;
            foreach (var word in words)
            {
                var wordCharCount = word.Length;
                if (wordCharCount >= freeSpace)
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
            { ColumnsEnum.Alias, 20 },
            { ColumnsEnum.ParamSlashArgs, 20 },
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

    public string[] GetAllowedArgs() => new string[] { "<any command>" };

    public string GetDescription() => "Prints information about all available commands";

    private void PrintCommandHelp()
    {

    }

    private void PrintLetterHelp(char letter)
    {

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
            Log.Trace(printIt);
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
            commandArgs = argumented.GetAllowedArgs();
        }

        string[] commandParams = null;
        if (cmd.IsParametrized())
        {
            var parametrized = cmd as IParametrized;
            commandParams = parametrized.GetAllowedParams();
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
