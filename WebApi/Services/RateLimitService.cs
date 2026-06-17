using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WebApi.Models;

namespace WebApi.Services
{
    public class RateLimitService
    {
        private readonly Dictionary<string, RateLimitEntry> _entries
            = new Dictionary<string, RateLimitEntry>(StringComparer.OrdinalIgnoreCase);

        private readonly int _defaultLimit;
        private readonly int _defaultWindowSeconds;

        public RateLimitService(int defaultLimit = 100, int defaultWindowSeconds = 60)
        {
            _defaultLimit = defaultLimit;
            _defaultWindowSeconds = defaultWindowSeconds;
        }

        public RateLimitResult Check(string key, int? limit = null, int? windowSeconds = null)
        {
            var actualLimit = limit ?? _defaultLimit;
            var actualWindow = windowSeconds ?? _defaultWindowSeconds;

            lock (_entries)
            {
                if (!_entries.TryGetValue(key, out var entry) || entry.WindowStart.AddSeconds(actualWindow) < DateTime.UtcNow)
                {
                    entry = new RateLimitEntry
                    {
                        Count = 0,
                        WindowStart = DateTime.UtcNow
                    };
                }

                entry.Count++;
                _entries[key] = entry;

                var remaining = Math.Max(0, actualLimit - entry.Count);
                var allowed = entry.Count <= actualLimit;

                return new RateLimitResult
                {
                    Allowed = allowed,
                    Limit = actualLimit,
                    Remaining = remaining,
                    Current = entry.Count,
                    WindowStart = entry.WindowStart,
                    ResetAt = entry.WindowStart.AddSeconds(actualWindow),
                    Key = key
                };
            }
        }

        public void Reset(string key)
        {
            lock (_entries)
            {
                _entries.Remove(key);
            }
        }

        public void ResetAll()
        {
            lock (_entries)
            {
                _entries.Clear();
            }
        }

        public List<Dictionary<string, object>> GetAllKeys()
        {
            lock (_entries)
            {
                return _entries.Select(kv => new Dictionary<string, object>
                {
                    ["key"] = kv.Key,
                    ["count"] = kv.Value.Count,
                    ["windowStart"] = kv.Value.WindowStart
                }).ToList();
            }
        }
    }

    public class RateLimitEntry
    {
        public int Count { get; set; }
        public DateTime WindowStart { get; set; }
    }

    public class RateLimitResult
    {
        public bool Allowed { get; set; }
        public int Limit { get; set; }
        public int Remaining { get; set; }
        public int Current { get; set; }
        public DateTime WindowStart { get; set; }
        public DateTime ResetAt { get; set; }
        public string Key { get; set; }
    }
}
