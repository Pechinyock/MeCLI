namespace Me;

public interface IFileSystem
{
    void Create(string path, string fileName);
    bool IsExists(string path);
}
