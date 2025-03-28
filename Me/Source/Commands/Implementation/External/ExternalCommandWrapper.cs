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
    private bool _isDone = false;
    private string _resultOutput;

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
        try
        {
            var proc = Process.Start(processStartInfo);
            _resultOutput = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();
            _isDone = true;
        }
        catch (Exception ex)
        {
            var c = ex;
            _isDone = true;
            _resultOutput = "error";
        }
        
    }

    public override bool Validate() => true;

    public string GetOutputRestlt() => _resultOutput;

    public bool IsCommandDone() => _isDone;
}