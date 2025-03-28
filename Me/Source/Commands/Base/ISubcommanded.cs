namespace Me;

public interface ISubcommanded
{
    void SetSubcommand(string[] subCommands);
    Dictionary<string, string> GetAvailableSubcommands();
}
