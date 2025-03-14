namespace Me;

public abstract class Librarian
{
    public static MeCommandBase Request(string alias) 
    {
        if(String.IsNullOrEmpty(alias))
            return null;

        var tolower = alias.ToLower();
        var fstChar = tolower[0];

        if (!LibraryMap.ContainsKey(fstChar))
            return null;

        var commandShelf = LibraryMap[fstChar];

        if (!commandShelf.ContainsKey(tolower))
            return null;

        return commandShelf[tolower];
    }

    public static MeCommandBase[] Request(char letter) 
    {
        if (!LibraryMap.ContainsKey(letter))
            return null;

        return LibraryMap[letter].Values.ToArray();
    }

    public static MeCommandBase[] RequestAll() 
    {
        int totalElement = 0;
        foreach (var shelf in LibraryMap) 
        {
            totalElement += shelf.Value.Count;
        }
        var result = new MeCommandBase[totalElement];
        for (int i = 0; i < totalElement; )
        {
            foreach (var shelf in LibraryMap) 
            {
                foreach (var command in shelf.Value) 
                {
                    result[i] = command.Value;
                    ++i;
                }
            }
        }
        return result;
    }

    private static Dictionary<char, Dictionary<string, MeCommandBase>> LibraryMap = new() 
    {
        { 'h', H.Get() },
        { 't', T.Get() },
        { 'e', E.Get() },
    };
}
