using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WebApi.Models;

namespace WebApi.Services
{
    public class GenericCrudService
    {
        private static readonly ConcurrentDictionary<string, List<Dictionary<string, object>>> _stores
            = new ConcurrentDictionary<string, List<Dictionary<string, object>>>(StringComparer.OrdinalIgnoreCase);

        private static readonly object _lock = new object();

        public List<Dictionary<string, object>> GetAll(string tableName)
        {
            var store = _stores.GetOrAdd(tableName, _ => new List<Dictionary<string, object>>());
            lock (_lock)
            {
                return store.ToList().Select(r => new Dictionary<string, object>(r)).ToList();
            }
        }

        public PagedResult<Dictionary<string, object>> GetPaged(string tableName, int page, int pageSize, string keyword = null, string keywordField = null)
        {
            var all = GetAll(tableName);
            if (!string.IsNullOrEmpty(keyword) && !string.IsNullOrEmpty(keywordField))
            {
                all = all.Where(r => r.ContainsKey(keywordField) &&
                    r[keywordField]?.ToString().Contains(keyword, StringComparison.OrdinalIgnoreCase) == true)
                    .ToList();
            }
            var total = all.Count;
            var items = all.Skip((page - 1) * pageSize).Take(pageSize)
                .Select(r => new Dictionary<string, object>(r)).ToList();
            return new PagedResult<Dictionary<string, object>>
            {
                Items = items,
                Total = total,
                Page = page,
                PageSize = pageSize
            };
        }

        public Dictionary<string, object> GetById(string tableName, string id)
        {
            if (!_stores.TryGetValue(tableName, out var store)) return null;
            lock (_lock)
            {
                var item = store.FirstOrDefault(r =>
                    r.ContainsKey("id") && r["id"]?.ToString() == id);
                return item != null ? new Dictionary<string, object>(item) : null;
            }
        }

        public Dictionary<string, object> Create(string tableName, Dictionary<string, object> data)
        {
            var store = _stores.GetOrAdd(tableName, _ => new List<Dictionary<string, object>>());
            var newData = new Dictionary<string, object>(data, StringComparer.OrdinalIgnoreCase);
            if (!newData.ContainsKey("id") || string.IsNullOrEmpty(newData["id"]?.ToString()))
            {
                newData["id"] = Guid.NewGuid().ToString("N");
            }
            newData["createdAt"] = DateTime.Now;
            newData["updatedAt"] = DateTime.Now;
            lock (_lock)
            {
                store.Add(newData);
            }
            return new Dictionary<string, object>(newData);
        }

        public bool Update(string tableName, string id, Dictionary<string, object> data)
        {
            if (!_stores.TryGetValue(tableName, out var store)) return false;
            lock (_lock)
            {
                var index = store.FindIndex(r =>
                    r.ContainsKey("id") && r["id"]?.ToString() == id);
                if (index < 0) return false;
                foreach (var kv in data)
                {
                    if (kv.Key.Equals("id", StringComparison.OrdinalIgnoreCase)) continue;
                    store[index][kv.Key] = kv.Value;
                }
                store[index]["updatedAt"] = DateTime.Now;
                return true;
            }
        }

        public bool Delete(string tableName, string id)
        {
            if (!_stores.TryGetValue(tableName, out var store)) return false;
            lock (_lock)
            {
                return store.RemoveAll(r =>
                    r.ContainsKey("id") && r["id"]?.ToString() == id) > 0;
            }
        }

        public List<string> GetTables()
        {
            return _stores.Keys.ToList();
        }

        public bool TableExists(string tableName)
        {
            return _stores.ContainsKey(tableName);
        }

        public long Count(string tableName)
        {
            if (!_stores.TryGetValue(tableName, out var store)) return 0;
            lock (_lock)
            {
                return store.Count;
            }
        }

        public bool BatchDelete(string tableName, List<string> ids)
        {
            if (!_stores.TryGetValue(tableName, out var store)) return false;
            lock (_lock)
            {
                return store.RemoveAll(r =>
                    r.ContainsKey("id") && ids.Contains(r["id"]?.ToString())) > 0;
            }
        }

        public bool BatchCreate(string tableName, List<Dictionary<string, object>> dataList)
        {
            var store = _stores.GetOrAdd(tableName, _ => new List<Dictionary<string, object>>());
            lock (_lock)
            {
                foreach (var data in dataList)
                {
                    var newData = new Dictionary<string, object>(data, StringComparer.OrdinalIgnoreCase);
                    if (!newData.ContainsKey("id") || string.IsNullOrEmpty(newData["id"]?.ToString()))
                    {
                        newData["id"] = Guid.NewGuid().ToString("N");
                    }
                    newData["createdAt"] = DateTime.Now;
                    newData["updatedAt"] = DateTime.Now;
                    store.Add(newData);
                }
            }
            return true;
        }
    }
}
