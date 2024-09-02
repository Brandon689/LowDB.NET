//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text.Json;

//namespace LowDBNet
//{
//    public class Collection<T> where T : class
//    {
//        private readonly string _name;
//        private readonly LowDB _db;
//        private List<T> _items;

//        public Collection(string name, LowDB db, List<T> items = null)
//        {
//            _name = name;
//            _db = db;
//            _items = items ?? new List<T>();
//        }

//        public IEnumerable<T> Find(Func<T, bool> predicate)
//        {
//            return _items.Where(predicate);
//        }

//        public void Insert(T item)
//        {
//            _items.Add(item);
//            _db.Write();
//        }

//        public void Update(Func<T, bool> predicate, T newItem)
//        {
//            var index = _items.FindIndex(item => predicate(item));
//            if (index != -1)
//            {
//                _items[index] = newItem;
//                _db.Write();
//            }
//        }

//        public void Remove(Func<T, bool> predicate)
//        {
//            _items.RemoveAll(item => predicate(item));
//            _db.Write();
//        }

//        public T FirstOrDefault(Func<T, bool> predicate)
//        {
//            return _items.FirstOrDefault(predicate);
//        }

//        public IEnumerable<T> Take(int count)
//        {
//            return _items.Take(count);
//        }

//        public IEnumerable<T> OrderBy<TKey>(Func<T, TKey> keySelector)
//        {
//            return _items.OrderBy(keySelector);
//        }
            
//        public IEnumerable<T> OrderByDescending<TKey>(Func<T, TKey> keySelector)
//        {
//            return _items.OrderByDescending(keySelector);
//        }
//    }
//}
