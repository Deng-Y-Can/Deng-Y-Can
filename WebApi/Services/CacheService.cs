using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;
using WebApi.Models;

namespace WebApi.Services
{
    public class CacheService
    {
        private readonly IMemoryCache _cache;
        private readonly ConcurrentDictionary<string, DateTime> _expiryMap
            = new ConcurrentDictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);

        public CacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void Set(string key, object value, TimeSpan? expiry = null)
        {
            var options = new MemoryCacheEntryOptions();
            if (expiry.HasValue)
            {
                options.AbsoluteExpirationRelativeToNow = expiry;
                _expiryMap[key] = DateTime.Now.Add(expiry.Value);
            }
            else
            {
                _expiryMap[key] = DateTime.MaxValue;
            }
            _cache.Set(key, value, options);
        }

        public object Get(string key)
        {
            return _cache.TryGetValue(key, out var value) ? value : null;
        }

        public T Get<T>(string key)
        {
            return _cache.TryGetValue(key, out var value) ? (T)value : default;
        }

        public bool Delete(string key)
        {
            _cache.Remove(key);
            _expiryMap.TryRemove(key, out _);
            return true;
        }

        public List<Dictionary<string, object>> GetAllKeys()
        {
            return _expiryMap.Select(kv => new Dictionary<string, object>
            {
                ["key"] = kv.Key,
                ["expiresAt"] = kv.Value == DateTime.MaxValue ? (object)"never" : kv.Value,
                ["exists"] = _cache.TryGetValue(kv.Key, out _)
            }).ToList();
        }

        public bool Exists(string key)
        {
            return _cache.TryGetValue(key, out _);
        }

        public long Count()
        {
            return _expiryMap.Count;
        }

        public void Clear()
        {
            foreach (var key in _expiryMap.Keys.ToList())
            {
                _cache.Remove(key);
            }
            _expiryMap.Clear();
        }
    }
}
