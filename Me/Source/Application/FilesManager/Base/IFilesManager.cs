namespace Me;

public enum IOResultEnum 
{
    Success             = 0,
    WrongFormat         = 1,
    PermissionDenied    = 2,
    PathTooLong         = 3,
    AlreadyExist        = 4
}

public interface IFilesManager
{
    string GetAppDataPath();
    IOResultEnum CreateDirectory(string path);
    IOResultEnum CreateFile(string path, string fileName);
    bool IsDirectiryExists(string path);
    bool IsFileExists(string path);
    string ReadAllText(string path);
    void WriteAllText(string path, string text);
}
