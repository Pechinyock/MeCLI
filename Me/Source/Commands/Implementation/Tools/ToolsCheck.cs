using System.Diagnostics;

namespace Me;

internal static class ToolsCheck
{
    private static readonly string _defaultInstallChecker = "--version";

    private static readonly Dictionary<string, string> _argumentChecker = new()
    {
        /* dotnet */ { Tools._namesOfRequiredTools[0], _defaultInstallChecker },
        /*    git */ { Tools._namesOfRequiredTools[1], _defaultInstallChecker },
        /* docker */ { Tools._namesOfRequiredTools[2], _defaultInstallChecker },
    };

    private static readonly Dictionary<string, Action<string>> _toolsMinVersionCheck = new()
    {
        /* dotnet */ { Tools._namesOfRequiredTools[0], CheckDotNetMinimalVersion },
        /*    git */ { Tools._namesOfRequiredTools[1], CheckGitMinimalVersion },
        /* docker */ { Tools._namesOfRequiredTools[2], CheckDockerMinimalVersion },
    };

    public static void Do() 
    {
        var externalCmd = new ExternalCommandWrapper();

        foreach (var tool in Tools._namesOfRequiredTools)
        {
            externalCmd.SetProgramName(tool);
            var checkArg = _argumentChecker[tool];
            externalCmd.SetArguments(checkArg);

            externalCmd.Execute();

            while (!externalCmd.IsCommandDone())
            {
                Task.Delay(1000);
            }

            var result = externalCmd.GetOutputRestlt();
            if (result == "error")
            {
                Print.Error($"The tool: '{tool}' is not available." +
                    $" It might be not installed or hasn't be added to Path variable");

                continue;
            }

            Debug.Assert(_toolsMinVersionCheck.ContainsKey(tool));

            var minVersionCheckLogic = _toolsMinVersionCheck[tool];

            minVersionCheckLogic.Invoke(result);
        }
    }

    private static void CheckDotNetMinimalVersion(string checkVersionOutput)
    {
        var dotnetInstalledVersion = ExternalAppVersion.Parse(checkVersionOutput);

        var minRequired = new ExternalAppVersion()
        {
            Major = 9,
            Minor = 0,
            Revision = 102,
        };

        if (minRequired > dotnetInstalledVersion)
        {
            Print.Warn($"{Tools._namesOfRequiredTools[0]}: installed version is lower than required!" +
                $" Please update {Tools._namesOfRequiredTools[0]} at lease on version {minRequired.ToString()}." +
                $" Install version is: {dotnetInstalledVersion}");

            return;
        }

        Print.Info($"{Tools._namesOfRequiredTools[0]}: " +
            $"{dotnetInstalledVersion.Major}" +
            $".{dotnetInstalledVersion.Minor}" +
            $".{dotnetInstalledVersion.Revision}"
        );
    }

    /* [TODO]
     * Linux and windows are different here I guess 
     * Should be conditional compiling
     */
    private static void CheckGitMinimalVersion(string checkVersionOutput)
    {
        var gitIsntalledVersion = ExternalAppVersion.Parse(checkVersionOutput);

        var minRequired = new ExternalAppVersion()
        {
            Major = 2,
            Minor = 30,
            Revision = 0,
        };

        if (minRequired > gitIsntalledVersion)
        {
            Print.Warn($"{Tools._namesOfRequiredTools[1]}: installed version is lower than required!" +
                $" Please update {Tools._namesOfRequiredTools[1]} at lease on version {minRequired.ToString()}." +
                $" Install version is: {gitIsntalledVersion}");

            return;
        }

        Print.Info($"{Tools._namesOfRequiredTools[1]}: " +
            $"{gitIsntalledVersion.Major}" +
            $".{gitIsntalledVersion.Minor}" +
            $".{gitIsntalledVersion.Revision}"
        );
    }

    /* [TODO]
     * Linux and windows are different here I guess 
     * Should be conditional compiling
     */
    private static void CheckDockerMinimalVersion(string checkVersionOutput)
    {
        var dockerInstalledVersion = ExternalAppVersion.Parse(checkVersionOutput);
        var minRequired = new ExternalAppVersion()
        {
            Major = 25,
            Minor = 0,
            Revision = 1,
        };

        if (minRequired > dockerInstalledVersion)
        {
            Print.Warn($"{Tools._namesOfRequiredTools[2]}: installed version is lower than required!" +
                $" Please update {Tools._namesOfRequiredTools[2]} at lease on version {minRequired.ToString()}." +
                $" Install version is: {dockerInstalledVersion}");

            return;
        }

        Print.Info($"{Tools._namesOfRequiredTools[2]}: " +
            $"{dockerInstalledVersion.Major}" +
            $".{dockerInstalledVersion.Minor}" +
            $".{dockerInstalledVersion.Revision}"
        );
    }
}
