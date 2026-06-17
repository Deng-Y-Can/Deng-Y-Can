using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WebApi.Services
{
    public class IpService
    {
        private static readonly HttpClient _http = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };

        private static readonly string[] _chinaPrefixes = {
            "10.", "172.16.", "172.17.", "172.18.", "172.19.",
            "172.20.", "172.21.", "172.22.", "172.23.", "172.24.",
            "172.25.", "172.26.", "172.27.", "172.28.", "172.29.",
            "172.30.", "172.31.", "192.168."
        };

        public async Task<Dictionary<string, object>> GetIpInfo(string ip = null)
        {
            var targetIp = ip;
            if (string.IsNullOrEmpty(targetIp))
            {
                try
                {
                    targetIp = await _http.GetStringAsync("https://api.ipify.org");
                }
                catch
                {
                    targetIp = "unknown";
                }
            }

            var result = new Dictionary<string, object>
            {
                ["ip"] = targetIp,
                ["isLocal"] = targetIp == "127.0.0.1" || targetIp == "::1" || targetIp.StartsWith("192.168.") || targetIp.StartsWith("10."),
                ["isPrivate"] = IsPrivateIp(targetIp)
            };

            try
            {
                var response = await _http.GetStringAsync($"https://ipapi.co/{targetIp}/json/");
                var obj = Newtonsoft.Json.Linq.JObject.Parse(response);
                result["country"] = obj["country_name"]?.ToString();
                result["region"] = obj["region"]?.ToString();
                result["city"] = obj["city"]?.ToString();
                result["latitude"] = obj["latitude"]?.ToString();
                result["longitude"] = obj["longitude"]?.ToString();
                result["timezone"] = obj["timezone"]?.ToString();
                result["org"] = obj["org"]?.ToString();
                result["postal"] = obj["postal"]?.ToString();
            }
            catch
            {
                result["geoInfo"] = "unavailable";
            }

            return result;
        }

        public string GetClientIp(HttpContext context)
        {
            var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(ip))
                ip = context.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (string.IsNullOrEmpty(ip))
                ip = context.Connection.RemoteIpAddress?.ToString();
            return ip;
        }

        public Dictionary<string, object> ResolveDns(string hostname)
        {
            var result = new Dictionary<string, object>
            {
                ["hostname"] = hostname,
                ["addresses"] = new List<string>()
            };

            try
            {
                var entries = Dns.GetHostAddresses(hostname);
                result["addresses"] = entries.Select(a => a.ToString()).ToList();
                result["addressFamily"] = entries.FirstOrDefault()?.AddressFamily.ToString();
            }
            catch (Exception ex)
            {
                result["error"] = ex.Message;
            }

            try
            {
                var entry = Dns.GetHostEntry(hostname);
                result["aliases"] = entry.Aliases?.ToList() ?? new List<string>();
                result["officialName"] = entry.HostName;
            }
            catch { }

            return result;
        }

        public async Task<List<Dictionary<string, object>>> ScanPorts(string host, int startPort = 1, int endPort = 100, int timeoutMs = 200)
        {
            var results = new List<Dictionary<string, object>>();
            var tasks = new List<Task>();

            for (int port = startPort; port <= endPort; port++)
            {
                var p = port;
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        using var client = new System.Net.Sockets.TcpClient();
                        var connectTask = client.ConnectAsync(host, p);
                        var timeoutTask = Task.Delay(timeoutMs);
                        if (await Task.WhenAny(connectTask, timeoutTask) == connectTask)
                        {
                            lock (results)
                            {
                                results.Add(new Dictionary<string, object>
                                {
                                    ["port"] = p,
                                    ["status"] = "open",
                                    ["service"] = GetServiceName(p)
                                });
                            }
                        }
                    }
                    catch { }
                }));
            }

            await Task.WhenAll(tasks);
            return results.OrderBy(r => r["port"]).ToList();
        }

        private bool IsPrivateIp(string ip)
        {
            if (string.IsNullOrEmpty(ip)) return false;
            return _chinaPrefixes.Any(p => ip.StartsWith(p)) || ip == "127.0.0.1" || ip == "::1";
        }

        private string GetServiceName(int port)
        {
            return port switch
            {
                21 => "FTP",
                22 => "SSH",
                23 => "Telnet",
                25 => "SMTP",
                53 => "DNS",
                80 => "HTTP",
                110 => "POP3",
                143 => "IMAP",
                443 => "HTTPS",
                3306 => "MySQL",
                3389 => "RDP",
                5432 => "PostgreSQL",
                6379 => "Redis",
                8080 => "HTTP-Alt",
                8443 => "HTTPS-Alt",
                27017 => "MongoDB",
                _ => "unknown"
            };
        }
    }
}
