using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApi.Models;

namespace WebApi.Services
{
    public class AuditService
    {
        private readonly ConcurrentBag<AuditLog> _logs = new ConcurrentBag<AuditLog>();
        private readonly int _maxLogs;

        public AuditService(int maxLogs = 10000)
        {
            _maxLogs = maxLogs;
        }

        public void AddLog(AuditLog log)
        {
            _logs.Add(log);
        }

        public PagedResult<AuditLog> GetLogs(int page = 1, int pageSize = 20, string method = null, string path = null)
        {
            var query = _logs.AsEnumerable();

            if (!string.IsNullOrEmpty(method))
                query = query.Where(l => l.Method.Equals(method, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(path))
                query = query.Where(l => l.Path.Contains(path, StringComparison.OrdinalIgnoreCase));

            var sorted = query.OrderByDescending(l => l.CreatedAt).ToList();
            var total = sorted.Count;
            var items = sorted.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return new PagedResult<AuditLog>
            {
                Items = items,
                Total = total,
                Page = page,
                PageSize = pageSize
            };
        }

        public void Clear()
        {
            while (_logs.TryTake(out _)) { }
        }

        public Dictionary<string, object> GetStats()
        {
            var logs = _logs.ToList();
            return new Dictionary<string, object>
            {
                ["totalRequests"] = logs.Count,
                ["avgResponseTime"] = logs.Any() ? logs.Average(l => l.ElapsedMs) : 0,
                ["errorCount"] = logs.Count(l => l.StatusCode >= 400),
                ["methodStats"] = logs.GroupBy(l => l.Method)
                    .ToDictionary(g => g.Key, g => (object)g.Count()),
                ["recentErrors"] = logs.Where(l => l.StatusCode >= 400)
                    .OrderByDescending(l => l.CreatedAt)
                    .Take(10)
                    .ToList()
            };
        }
    }
}
