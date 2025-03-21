namespace Me;

public interface IParametrized
{
    public void SetParameters(Dictionary<string, string> value);

    public Dictionary<string, string> GetPassedParameters();

    public Dictionary<string, string> GetAvailableParameters();

    public string GetParameterIndicator();

    public string[] GetParamsWithDescription();
}
