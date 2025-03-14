namespace Me;

#region Extensions

public static class EnumTypeExtensions
{
    public static bool IsArgumented(this MeCommandBase cmd)
    {
        return cmd.GetTypes.HasFlag(CmdTypeEnumFlag.Argumented);
    }

    public static bool IsExternal(this MeCommandBase cmd)
    {
        return cmd.GetTypes.HasFlag(CmdTypeEnumFlag.External);
    }

    public static bool IsParametrized(this MeCommandBase cmd)
    {
        return cmd.GetTypes.HasFlag(CmdTypeEnumFlag.Parzmetrized);
    }

    public static bool IsDescribed(this MeCommandBase cmd)
    {
        return cmd.GetTypes.HasFlag(CmdTypeEnumFlag.Described);
    }
}

#endregion

public enum CmdTypeEnumFlag
{
    None = 0,
    Described = 1 << 0,
    Parzmetrized = 1 << 1,
    Argumented = 1 << 2,
    External = 1 << 3,
}

public abstract class MeCommandBase
{
    public string[] Input { get; set; }

    public string Arguments { get; protected set; }

    public string Parameters { get; protected set; }

    public abstract string Alias { get; }

    public abstract CmdTypeEnumFlag GetTypes { get; }

    public abstract void Execute();
}
