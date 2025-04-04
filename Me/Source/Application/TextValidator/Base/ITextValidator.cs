namespace Me;

public enum ValidationOptions 
{
    IgnoreCase = 0
}

public interface ITextValidator
{
    bool IsMatch(string text, string pattern, ValidationOptions options = ValidationOptions.IgnoreCase);
}
