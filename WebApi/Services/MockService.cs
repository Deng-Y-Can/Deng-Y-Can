using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WebApi.Services
{
    public class MockService
    {
        private readonly ConcurrentDictionary<string, MockEndpoint> _endpoints
            = new ConcurrentDictionary<string, MockEndpoint>(StringComparer.OrdinalIgnoreCase);

        public MockEndpoint Register(string path, string method, object responseBody, int statusCode = 200,
            int delayMs = 0, Dictionary<string, string> headers = null)
        {
            var endpoint = new MockEndpoint
            {
                Path = path,
                Method = method?.ToUpper() ?? "GET",
                ResponseBody = responseBody,
                StatusCode = statusCode,
                DelayMs = delayMs,
                Headers = headers ?? new Dictionary<string, string>(),
                CallCount = 0,
                CreatedAt = DateTime.Now
            };
            _endpoints[path.ToUpper()] = endpoint;
            return endpoint;
        }

        public MockEndpoint Match(string path, string method)
        {
            if (_endpoints.TryGetValue(path.ToUpper(), out var endpoint))
            {
                if (endpoint.Method == "ANY" || endpoint.Method == method?.ToUpper())
                {
                    endpoint.CallCount++;
                    endpoint.LastCalledAt = DateTime.Now;
                    return endpoint;
                }
            }
            return null;
        }

        public bool Remove(string path)
        {
            return _endpoints.TryRemove(path.ToUpper(), out _);
        }

        public List<MockEndpoint> GetAll()
        {
            return _endpoints.Values.ToList();
        }

        public void Clear()
        {
            _endpoints.Clear();
        }
    }

    public class MockEndpoint
    {
        public string Path { get; set; }
        public string Method { get; set; }
        public object ResponseBody { get; set; }
        public int StatusCode { get; set; }
        public int DelayMs { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public int CallCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastCalledAt { get; set; }
    }
}
