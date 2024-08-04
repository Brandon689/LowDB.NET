namespace LowDBNet
{
    public class InMemoryAdapter : IStorageAdapter
    {
        private string _data = string.Empty;

        public string Read()
        {
            return _data;
        }

        public void Write(string data)
        {
            _data = data;
        }
    }
}
