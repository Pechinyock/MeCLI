namespace Me;

internal abstract class E
{
    public static Dictionary<string, MeCommandBase> Get() => Shelf;

    private static Dictionary<string, MeCommandBase> Shelf = new()
    {
    };
}
