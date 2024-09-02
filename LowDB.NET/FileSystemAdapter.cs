using System;
using System.IO;

namespace LowDBNet
{
    public class FileSystemAdapter : IStorageAdapter
    {
        private readonly string _filePath;

        public FileSystemAdapter(string filePath)
        {
            _filePath = filePath;
        }

        public string Read()
        {
            if (!File.Exists(_filePath))
            {
                Console.WriteLine($"File not found: {_filePath}");
                return string.Empty;
            }
            var content = File.ReadAllText(_filePath);
            Console.WriteLine($"Read {content.Length} characters from file");
            return content;
        }

        public void Write(string data)
        {
            string tempPath = Path.GetTempFileName();
            File.WriteAllText(tempPath, data);
            File.Move(tempPath, _filePath, true);
            Console.WriteLine($"Wrote {data.Length} characters to file");
        }
    }
}
