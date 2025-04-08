namespace Me;

internal class FilesManager : IFilesManager
{
    public IOResultEnum CreateDirectory(string path) 
    {
        try
        {
            if(Path.Exists(path))
                return IOResultEnum.AlreadyExist;

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

    public string GetAppDataPath()
    {
        var specialDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData
            , Environment.SpecialFolderOption.Create
        );
        var appDataPath = Path.Combine(specialDirectoryPath, "me");

        if (IsDirectiryExists(appDataPath))
            return appDataPath;
        else
        {
            CreateDirectory(appDataPath);
            return appDataPath;
        }
    }

    public bool IsDirectiryExists(string path)
    {
        return Directory.Exists(path);
    }
}
