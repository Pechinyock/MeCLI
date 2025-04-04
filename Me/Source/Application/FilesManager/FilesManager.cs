namespace Me;

internal class FilesManager : IFilesManager
{
    public IOResultEnum CreateDirectory(string path) 
    {
        try
        {
            Directory.CreateDirectory(path);
            return IOResultEnum.Success;
        }
        catch (ArgumentException)
        {
            return IOResultEnum.WrongFormat;
        }
        catch (UnauthorizedAccessException)
        {
            return IOResultEnum.PermissionDenied;
        }
        catch (PathTooLongException) 
        {
            return IOResultEnum.PathTooLong;
        }
    }

    public IOResultEnum CreateFile(string path, string fileName)
    {
        return IOResultEnum.Success;
    }

    public bool IsExists(string path)
    {
        throw new NotImplementedException();
    }
}
