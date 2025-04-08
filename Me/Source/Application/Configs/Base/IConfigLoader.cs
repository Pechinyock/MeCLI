namespace Me;

internal interface IConfigLoader
{
    TConfig Load<TConfig>(string name);
}
