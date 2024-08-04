using System;
using System.Collections.Generic;
using System.Linq;

namespace LowDBNet
{
    public class Collection<T> where T : class
    {
        private readonly string _name;
        private readonly LowDB _db;
        private readonly List<T> _items;
        private readonly object _lock = new object();

        public Collection(string name, LowDB db)
        {
            _name = name;
            _db = db;
            _items = new List<T>();
        }

        public IEnumerable<T> Find(Func<T, bool> predicate)
        {
            return _items.Where(predicate);
        }

        public void Insert(T item)
        {
            lock (_lock)
            {
                _items.Add(item);
                _db.Write();
            }
        }

        public void Update(Func<T, bool> predicate, T newItem)
        {
            var itemsToUpdate = _items.Where(predicate).ToList();
            foreach (var item in itemsToUpdate)
            {
                var index = _items.IndexOf(item);
                _items[index] = newItem;
            }
            _db.Write();
        }

        public void Remove(Func<T, bool> predicate)
        {
            _items.RemoveAll(item => predicate(item));
            _db.Write();
        }

        public T FirstOrDefault(Func<T, bool> predicate)
        {
            return _items.FirstOrDefault(predicate);
        }

        public IEnumerable<T> Take(int count)
        {
            return _items.Take(count);
        }

        public IEnumerable<T> OrderBy<TKey>(Func<T, TKey> keySelector)
        {
            return _items.OrderBy(keySelector);
        }

        public IEnumerable<T> OrderByDescending<TKey>(Func<T, TKey> keySelector)
        {
            return _items.OrderByDescending(keySelector);
        }
    }
}
