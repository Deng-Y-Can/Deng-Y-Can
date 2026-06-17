using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WebApi.Services
{
    public class HealthService
    {
        private readonly IHostApplicationLifetime _lifetime;
        private readonly ILogger<HealthService> _logger;
        private static readonly Stopwatch _uptime = Stopwatch.StartNew();

        public HealthService(IHostApplicationLifetime lifetime, ILogger<HealthService> logger)
        {
            _lifetime = lifetime;
            _logger = logger;
        }

        public Dictionary<string, object> GetHealth()
        {
            return new Dictionary<string, object>
            {
                ["status"] = "healthy",
                ["timestamp"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                ["uptime"] = _uptime.Elapsed.ToString(@"d\.hh\:mm\:ss"),
                ["uptimeMs"] = _uptime.ElapsedMilliseconds,
                ["system"] = GetSystemInfo(),
                ["process"] = GetProcessInfo(),
                ["memory"] = GetMemoryInfo(),
                ["disk"] = GetDiskInfo()
            };
        }

        public Dictionary<string, object> GetSystemInfo()
        {
            return new Dictionary<string, object>
            {
                ["osVersion"] = RuntimeInformation.OSDescription,
                ["osArchitecture"] = RuntimeInformation.OSArchitecture.ToString(),
                ["processorCount"] = Environment.ProcessorCount,
                ["frameworkDescription"] = RuntimeInformation.FrameworkDescription,
                ["machineName"] = Environment.MachineName,
                ["userName"] = Environment.UserName
            };
        }

        public Dictionary<string, object> GetProcessInfo()
        {
            var process = Process.GetCurrentProcess();
            return new Dictionary<string, object>
            {
                ["id"] = process.Id,
                ["name"] = process.ProcessName,
                ["cpuUsage"] = GetCpuUsage(process),
                ["threads"] = process.Threads.Count,
                ["startTime"] = process.StartTime.ToString("yyyy-MM-dd HH:mm:ss"),
                ["totalProcessorTime"] = process.TotalProcessorTime.ToString(@"hh\:mm\:ss")
            };
        }

        public Dictionary<string, object> GetMemoryInfo()
        {
            var process = Process.GetCurrentProcess();
            return new Dictionary<string, object>
            {
                ["workingSetMB"] = Math.Round(process.WorkingSet64 / 1024.0 / 1024.0, 2),
                ["privateMemoryMB"] = Math.Round(process.PrivateMemorySize64 / 1024.0 / 1024.0, 2),
                ["gcTotalMemoryMB"] = Math.Round(GC.GetTotalMemory(false) / 1024.0 / 1024.0, 2),
                ["gen0Collections"] = GC.CollectionCount(0),
                ["gen1Collections"] = GC.CollectionCount(1),
                ["gen2Collections"] = GC.CollectionCount(2)
            };
        }

        public List<Dictionary<string, object>> GetDiskInfo()
        {
            return DriveInfo.GetDrives()
                .Where(d => d.IsReady)
                .Select(d => new Dictionary<string, object>
                {
                    ["name"] = d.Name,
                    ["label"] = d.VolumeLabel,
                    ["format"] = d.DriveFormat,
                    ["totalGB"] = Math.Round(d.TotalSize / 1024.0 / 1024.0 / 1024.0, 2),
                    ["freeGB"] = Math.Round(d.AvailableFreeSpace / 1024.0 / 1024.0 / 1024.0, 2),
                    ["usedGB"] = Math.Round((d.TotalSize - d.AvailableFreeSpace) / 1024.0 / 1024.0 / 1024.0, 2),
                    ["usagePercent"] = d.TotalSize > 0
                        ? Math.Round((double)(d.TotalSize - d.AvailableFreeSpace) / d.TotalSize * 100, 1)
                        : 0
                })
                .ToList();
        }

        public async Task<List<Dictionary<string, object>>> PingAsync(List<string> hosts)
        {
            var results = new List<Dictionary<string, object>>();
            using var ping = new Ping();
            foreach (var host in hosts)
            {
                try
                {
                    var reply = await ping.SendPingAsync(host, 5000);
                    results.Add(new Dictionary<string, object>
                    {
                        ["host"] = host,
                        ["status"] = reply.Status.ToString(),
                        ["roundtripMs"] = reply.RoundtripTime,
                        ["ttl"] = reply.Options?.Ttl
                    });
                }
                catch (Exception ex)
                {
                    results.Add(new Dictionary<string, object>
                    {
                        ["host"] = host,
                        ["status"] = "Error",
                        ["error"] = ex.Message
                    });
                }
            }
            return results;
        }

        private double GetCpuUsage(Process process)
        {
            try
            {
                return Math.Round(process.TotalProcessorTime.TotalMilliseconds /
                    (DateTime.Now - process.StartTime).TotalMilliseconds /
                    Environment.ProcessorCount * 100, 2);
            }
            catch { return 0; }
        }
    }
}
