namespace Me;

internal abstract class T
{
    public static Dictionary<string, MeCommandBase> Get() => Shelf;

    private static Dictionary<string, MeCommandBase> Shelf = new()
    {
        { "tools", new Tools() },
    };
}
