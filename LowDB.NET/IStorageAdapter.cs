namespace LowDBNet
{
    public interface IStorageAdapter
    {
        string Read();
        void Write(string data);
    }
}
