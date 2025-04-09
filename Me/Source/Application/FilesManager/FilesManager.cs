using System.Diagnostics;

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
        try
        {
            var fullPath = Path.Combine(path, fileName);
            if (IsFileExists(fullPath))
            {

                return IOResultEnum.AlreadyExist;
            }

            File.Create(path);

            return IOResultEnum.Success;
        }
        catch (ArgumentNullException)
        {
            /*[TODO] come up with handle */
            return IOResultEnum.WrongFormat;
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
        catch (DirectoryNotFoundException)
        {
            /*[TODO] come up with handle */
            return IOResultEnum.WrongFormat;
        }
        catch (NotSupportedException) 
        {
            /*[TODO] come up with handle */
            return IOResultEnum.WrongFormat;
        }
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

    public bool IsFileExists(string path)
    {
        Debug.Assert(!String.IsNullOrWhiteSpace(path));
        return File.Exists(path);
    }

    public string ReadAllText(string path)
    {
        Debug.Assert(!String.IsNullOrEmpty(path));
        return File.ReadAllText(path);
    }

    public void WriteAllText(string path, string text)
    {
        Debug.Assert(!String.IsNullOrWhiteSpace(path));
        File.WriteAllText(path, text);
    }
}
