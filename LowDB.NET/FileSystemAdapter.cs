using LowDBNet;

public class FileSystemAdapter : IStorageAdapter
{
    private readonly string _filePath;

    public FileSystemAdapter(string filePath)
    {
        _filePath = filePath;
    }

    public string Read()
    {
        return File.Exists(_filePath) ? File.ReadAllText(_filePath) : string.Empty;
    }

    public void Write(string data)
    {
        string tempPath = Path.GetTempFileName();
        File.WriteAllText(tempPath, data);
        File.Move(tempPath, _filePath, true);
    }
}
