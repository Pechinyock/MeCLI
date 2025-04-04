using System.Text.RegularExpressions;

namespace Me;

internal class RegexValidator : ITextValidator
{
    public bool IsMatch(string text, string pattern, ValidationOptions options = ValidationOptions.IgnoreCase)
    {
        var regexOptions = MapOptions(options);
        var regex = new Regex(pattern, regexOptions);
        return false;
    }

    private static RegexOptions MapOptions(ValidationOptions opt) 
    {
        return opt switch
        {
            ValidationOptions.IgnoreCase => RegexOptions.IgnoreCase,
            _ => throw new ArgumentOutOfRangeException(),
        };
    }
}
