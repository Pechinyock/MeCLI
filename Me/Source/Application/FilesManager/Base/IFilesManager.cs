namespace Me;

public enum IOResultEnum 
{
    Success             = 0,
    WrongFormat         = 1,
    PermissionDenied    = 2,
    PathTooLong         = 3,
}

public interface IFilesManager
{
    IOResultEnum CreateDirectory(string path);
    IOResultEnum CreateFile(string path, string fileName);
    bool IsExists(string path);
}
