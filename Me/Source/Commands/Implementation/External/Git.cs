using System.Diagnostics;

namespace Me;
internal static class Git
{
    private static readonly string _gitAlias = "git";

    public static void Init(string path) 
    {
        Debug.Assert(!String.IsNullOrWhiteSpace(path));
        var current = Directory.GetCurrentDirectory();
        Directory.SetCurrentDirectory(path);
        var extWrapper = new ExternalCommandWrapper();
        extWrapper.SetProgramName(_gitAlias);
        extWrapper.SetArguments("init");
        extWrapper.Execute();
        var progress = extWrapper.IsCommandDone();
        while (!progress) 
        {
            Print.Info("waiting for git...");
            Thread.Sleep(100);
            progress = extWrapper.IsCommandDone();
        }
        /* [TODO] Remade after exterla cmd changes*/
        if (extWrapper.GetOutputRestlt() == "error") 
        {
            Print.Error("Failed to init git repository");
        }

        Directory.SetCurrentDirectory(current);
    }

}
