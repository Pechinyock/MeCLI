namespace Me;

public interface IParametrized
{
    void SetParameters(Dictionary<string, string> value);

    Dictionary<string, string> GetParameters();

    string[] GetParameterIndicators();

    string[] GetAllowedParams();
}
