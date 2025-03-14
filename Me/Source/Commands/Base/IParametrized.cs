namespace Me;

public interface IParametrized
{
    void SetParameters(Dictionary<string, string> value);

    string[] GetParameterIndicators();
}
