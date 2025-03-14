using System.Diagnostics;
using System.Text;

namespace Me;

internal sealed class Help : MeCommandBase
                           , IArgumented
{
    #region Nested
    private enum ColumnsEnum 
    {
        Alias = 0,
        ParamSlashArgs = 1,
        Description = 2
    }
    #endregion

    private Dictionary<ColumnsEnum, int> _columnsSizePresentage = new()
    {
        { ColumnsEnum.Alias, 15 },
        { ColumnsEnum.ParamSlashArgs, 40 },
        { ColumnsEnum.Description, 45 }
    };

    private string[] _passedArguments;

    public override string Alias => "help";

    public override CmdTypeEnumFlag GetTypes => CmdTypeEnumFlag.Argumented;

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

    private void PrintCommandHelp()
    {
        foreach (var arg in _passedArguments) 
        {
            var cmd = Librarian.Request(arg);
            if (cmd is null)
            { 
                Log.Error($"Command: '{arg}' not found");
                continue;
            }

            if (!cmd.IsDescribed())
                continue;

            var described = cmd as IDescribed;
            Debug.Assert(described != null);
            Console.WriteLine(described.GetDescription());
            var argsParamDescription = described.GetParamsArgsDescription();
            foreach (var paramOrArg in argsParamDescription) 
            {
                Console.WriteLine($"{paramOrArg.Key} - {paramOrArg.Value}");
            }
        }
    }

    private void PrintLetterHelp(char letter) 
    {
        var allCommandsWithLetter = Librarian.Request(letter);
        if (allCommandsWithLetter is null) 
        {
            Log.Error($"There's no commands that start with letter '{letter}'");
            return;
        }
        foreach (var cmd in allCommandsWithLetter) 
        {
            var described = cmd as IDescribed;
            Log.Trace(described.GetDescription());
            foreach (var paramOrArg in described.GetParamsArgsDescription()) 
            {
                Log.Trace($"{paramOrArg.Key} {paramOrArg.Value}");
            }
        }
    }

    private void PrintGlobalHelp()
    {
        var allAvailableCommands = Librarian.RequestAll();
        PrintCommandHelp(allAvailableCommands);
    }

    private void PrintCommandHelp(MeCommandBase[] cmd)
    {
        Console.Clear();

        var consoleWith = Console.WindowWidth;
        var fullConsolWindowLine = new StringBuilder();
        while (consoleWith > 0)
        {
            fullConsolWindowLine.Append("-");
            --consoleWith;
        }
        var aliasColumnWidth = CalculateColumnwidth(ColumnsEnum.Alias, Console.WindowWidth);
        var aliasSb = new StringBuilder();
        while (aliasColumnWidth > 0)
        {
            aliasSb.Append("-");
            --aliasColumnWidth;
        }
        Log.Trace(aliasSb.ToString());
    }

    private int CalculateColumnwidth(ColumnsEnum column, int width)
    {
        Debug.Assert(_columnsSizePresentage.ContainsKey(column));
        var precents = _columnsSizePresentage[column];

        var widthOut = (int)((width * precents) / 100);
        return widthOut;
    }

}
