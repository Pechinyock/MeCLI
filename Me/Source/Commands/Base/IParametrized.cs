namespace Me;

public interface IParametrized
{
    void SetParameters(Dictionary<string, string> value);

    Dictionary<string, string> GetPassedParameters();

    Dictionary<string, string> GetAvailableParameters();

    string GetParameterIndicator();

    string[] GetParamsWithDescription();
}
