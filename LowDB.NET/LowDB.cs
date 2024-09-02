using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace LowDBNet
{
    public class LowDB
    {
        private readonly IStorageAdapter _storage;
        private Dictionary<string, JsonElement> _data;

        public LowDB(string filePath)
        {
            try
            {
                _storage = new FileSystemAdapter(filePath);
                Console.WriteLine($"Using file storage: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to use file storage: {ex.Message}");
                Console.WriteLine("Falling back to in-memory storage");
                _storage = new InMemoryAdapter();
            }
            LoadData();
        }

        private void LoadData()
        {
            var json = _storage.Read();
            Console.WriteLine($"Read data from storage: {(string.IsNullOrEmpty(json) ? "empty" : $"{json.Length} characters")}");
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    using var document = JsonDocument.Parse(json);
                    _data = document.RootElement.EnumerateObject()
                        .ToDictionary(p => p.Name, p => p.Value.Clone());
                    Console.WriteLine($"Loaded {_data.Count} collections from JSON");
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Error parsing JSON: {ex.Message}");
                    _data = new Dictionary<string, JsonElement>();
                }
            }
            else
            {
                _data = new Dictionary<string, JsonElement>();
            }
        }

        public IEnumerable<T> GetCollection<T>(string name) where T : class
        {
            if (_data.TryGetValue(name, out var element))
            {
                return JsonSerializer.Deserialize<List<T>>(element.GetRawText(), new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<T>();
            }
            return new List<T>();
        }

        public void SaveChanges()
        {
            var json = JsonSerializer.Serialize(_data);
            _storage.Write(json);
            Console.WriteLine("Data written to storage");
        }

        public void AddToCollection<T>(string name, T item) where T : class
        {
            var collection = GetCollection<T>(name).ToList();
            collection.Add(item);
            UpdateCollection(name, collection);
        }

        public void UpdateInCollection<T>(string name, Func<T, bool> predicate, T newItem) where T : class
        {
            var collection = GetCollection<T>(name).ToList();
            var index = collection.FindIndex(item => predicate(item));
            if (index != -1)
            {
                collection[index] = newItem;
                UpdateCollection(name, collection);
            }
        }

        public void RemoveFromCollection<T>(string name, Func<T, bool> predicate) where T : class
        {
            var collection = GetCollection<T>(name).ToList();
            collection.RemoveAll(item => predicate(item));
            UpdateCollection(name, collection);
        }

        private void UpdateCollection<T>(string name, List<T> collection) where T : class
        {
            var json = JsonSerializer.Serialize(collection);
            _data[name] = JsonDocument.Parse(json).RootElement;
        }
    }
}


//using System;
//using System.Collections.Generic;
//using System.Text.Json;

//namespace LowDBNet
//{
//    public class LowDB
//    {
//        private readonly IStorageAdapter _storage;
//        private Dictionary<string, JsonElement> _data;

//        public LowDB(IStorageAdapter storage)
//        {
//            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
//            _data = new Dictionary<string, JsonElement>();
//            LoadData();
//        }

//        public LowDB(string filePath)
//        {
//            try
//            {
//                _storage = new FileSystemAdapter(filePath);
//                Console.WriteLine($"Using file storage: {filePath}");
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Failed to use file storage: {ex.Message}");
//                Console.WriteLine("Falling back to in-memory storage");
//                _storage = new InMemoryAdapter();
//            }
//            _data = new Dictionary<string, JsonElement>();
//            LoadData();
//        }

//        public Collection<T> GetCollection<T>(string name) where T : class
//        {
//            if (!_data.TryGetValue(name, out var jsonElement))
//            {
//                Console.WriteLine($"Collection '{name}' not found in the data");
//                return new Collection<T>(name, this);
//            }

//            Console.WriteLine($"Deserializing collection '{name}'");
//            var items = JsonSerializer.Deserialize<List<T>>(jsonElement.GetRawText());
//            Console.WriteLine($"Deserialized {items?.Count ?? 0} items for collection '{name}'");
//            return new Collection<T>(name, this, items);
//        }

//        private void LoadData()
//        {
//            var json = _storage.Read();
//            Console.WriteLine($"Read data from storage: {(string.IsNullOrEmpty(json) ? "empty" : $"{json.Length} characters")}");
//            if (!string.IsNullOrEmpty(json))
//            {
//                try
//                {
//                    using var document = JsonDocument.Parse(json);
//                    _data = new Dictionary<string, JsonElement>();
//                    foreach (var property in document.RootElement.EnumerateObject())
//                    {
//                        _data[property.Name] = property.Value.Clone();
//                    }
//                    Console.WriteLine($"Loaded {_data.Count} collections from JSON");
//                }
//                catch (JsonException ex)
//                {
//                    Console.WriteLine($"Error parsing JSON: {ex.Message}");
//                }
//            }
//        }

//        internal void Write()
//        {
//            var json = JsonSerializer.Serialize(_data);
//            _storage.Write(json);
//            Console.WriteLine("Data written to storage");
//        }
//    }
//}
