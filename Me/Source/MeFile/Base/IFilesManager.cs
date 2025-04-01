namespace Me;

public interface IFilesManager
{
    void Create(string path, string fileName);
    bool IsExists(string path);
}
