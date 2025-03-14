namespace Me;

internal abstract class H
{
    public static Dictionary<string, MeCommandBase> Get() => Shelf;

    private static Dictionary<string, MeCommandBase> Shelf = new() 
    {
        { "help", new Help() },
    };
}
