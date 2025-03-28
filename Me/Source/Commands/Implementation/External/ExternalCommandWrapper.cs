using System.Diagnostics;

namespace Me;

internal sealed class ExternalCommandWrapper : MeCommandBase
                                             , IExternal
{
    public override string Alias => "ext";
    public override CmdTypeEnumFlag GetTypes => CmdTypeEnumFlag.External;

    public void SetArguments(string value) => _arguments = value;
    public void SetProgramName(string value) => _programName = value;

    private string _arguments;
    private string _programName;

    public override void Execute()
    {
        var processStartInfo = new ProcessStartInfo()
        {
            FileName = _programName,
            Arguments = _arguments,
            CreateNoWindow = true,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
        };

        var proc = Process.Start(processStartInfo);
        string output = proc.StandardOutput.ReadToEnd();
        proc.WaitForExit();
    }

    public override bool Validate() => true;
}