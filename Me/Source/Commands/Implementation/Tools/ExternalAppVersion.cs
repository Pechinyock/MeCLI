using System.Diagnostics;

namespace Me;

internal class ExternalAppVersion
{
    private static readonly char _versionSeporator = '.';

    public int Major { get; set; }
    public int Minor { get; set; }
    public int Revision { get; set; }

    public override string ToString()
    {
        return $"{Major}{_versionSeporator}" +
            $"{Minor}{_versionSeporator}" +
            $"{Revision}";
    }

    public static ExternalAppVersion Parse(string sourceString)
    {
        Debug.Assert(!String.IsNullOrEmpty(sourceString));
        var versionString = String.Empty;
        foreach (var character in sourceString)
        {
            if (Char.IsDigit(character))
                versionString += character;

            if (character == _versionSeporator)
                versionString += character;
        }

        if (String.IsNullOrEmpty(versionString))
            return null;

        var splited = versionString.Split(_versionSeporator);

        var result = new ExternalAppVersion();

        result.Major = int.Parse(splited[0]);
        result.Minor = int.Parse(splited[1]);
        result.Revision = int.Parse(splited[2]);

        return result;
    }

    public static bool operator >(ExternalAppVersion a, ExternalAppVersion b)
    {
        if (a.Major > b.Major)
            return true;
        else if (a.Major == b.Major)
        {
            if (a.Minor > b.Minor)
                return true;
            else if (a.Minor == a.Minor)
            {
                if (a.Revision > b.Revision)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }
        else
            return false;
    }

    public static bool operator <(ExternalAppVersion a, ExternalAppVersion b)
    {
        return !(a > b);
    }
}