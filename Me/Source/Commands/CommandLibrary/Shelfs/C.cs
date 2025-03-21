namespace Me;

internal abstract class C
{
    public static Dictionary<string, MeCommandBase> Get() => Shelf;

    private static Dictionary<string, MeCommandBase> Shelf = new()
    {
        { "create", new Create() },
    };
}
