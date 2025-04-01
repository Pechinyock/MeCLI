using Me;

internal static class EntryPoint
{
    /* [TODO]
     * Create should be ISubcommanded, to resolve validation issue
     */

    public static void Main(string[] args)
    {
        var app = new Application();
        app.Run(args);
    }
}