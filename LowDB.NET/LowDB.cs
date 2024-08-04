using System;
using System.Collections.Generic;
using System.Text.Json;

namespace LowDBNet
{
    public class LowDB
    {
        private readonly IStorageAdapter _storage;
        private Dictionary<string, object> _data;

        public LowDB(IStorageAdapter storage)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _data = new Dictionary<string, object>();
            LoadData();
        }

        public LowDB(string filePath)
        {
            try
            {
                _storage = new FileSystemAdapter(filePath);
                // Test write
                _storage.Write("{}");
            }
            catch
            {
                // Fallback to in-memory storage
                _storage = new InMemoryAdapter();
            }
            _data = new Dictionary<string, object>();
            LoadData();
        }

        public Collection<T> GetCollection<T>(string name) where T : class
        {
            if (!_data.TryGetValue(name, out var collection))
            {
                collection = new Collection<T>(name, this);
                _data[name] = collection;
            }
            return (Collection<T>)collection;
        }

        private void LoadData()
        {
            var json = _storage.Read();
            if (!string.IsNullOrEmpty(json))
            {
                _data = JsonSerializer.Deserialize<Dictionary<string, object>>(json)
                    ?? new Dictionary<string, object>();
            }
        }

        internal void Write()
        {
            var json = JsonSerializer.Serialize(_data);
            _storage.Write(json);
        }
    }
}
