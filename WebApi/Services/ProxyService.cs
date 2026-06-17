using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WebApi.Services
{
    public class ProxyService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProxyService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<Dictionary<string, object>> ForwardAsync(
            string method, string url, Dictionary<string, string> headers = null,
            string body = null, int timeoutSeconds = 30)
        {
            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);

            var request = new HttpRequestMessage(new HttpMethod(method), url);

            if (headers != null)
            {
                foreach (var kv in headers)
                    request.Headers.TryAddWithoutValidation(kv.Key, kv.Value);
            }

            if (!string.IsNullOrEmpty(body) && (method.Equals("POST", StringComparison.OrdinalIgnoreCase) ||
                method.Equals("PUT", StringComparison.OrdinalIgnoreCase)))
            {
                request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            }

            var sw = Stopwatch.StartNew();
            try
            {
                var response = await client.SendAsync(request);
                sw.Stop();
                var responseBody = await response.Content.ReadAsStringAsync();

                return new Dictionary<string, object>
                {
                    ["statusCode"] = (int)response.StatusCode,
                    ["statusText"] = response.ReasonPhrase,
                    ["elapsedMs"] = sw.ElapsedMilliseconds,
                    ["headers"] = response.Headers.ToDictionary(h => h.Key, h => string.Join(", ", h.Value)),
                    ["body"] = responseBody
                };
            }
            catch (Exception ex)
            {
                sw.Stop();
                return new Dictionary<string, object>
                {
                    ["error"] = ex.Message,
                    ["elapsedMs"] = sw.ElapsedMilliseconds
                };
            }
        }

        public async Task<Dictionary<string, object>> GetAsync(string url, Dictionary<string, string> headers = null)
        {
            return await ForwardAsync("GET", url, headers);
        }

        public async Task<Dictionary<string, object>> PostAsync(string url, string body, Dictionary<string, string> headers = null)
        {
            return await ForwardAsync("POST", url, headers, body);
        }
    }
}
