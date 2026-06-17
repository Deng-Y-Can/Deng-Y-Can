using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using WebApi.Models;

namespace WebApi.Services
{
    public class SmsService
    {
        private readonly ConcurrentBag<SmsMessage> _sentMessages = new ConcurrentBag<SmsMessage>();

        public Task SendAsync(SmsMessage message)
        {
            _sentMessages.Add(message);
            Debug.WriteLine($"[SMS] To: {message.Phone}, Content: {message.Content}");
            return Task.CompletedTask;
        }

        public List<SmsMessage> GetHistory(int limit = 100)
        {
            return _sentMessages.OrderByDescending(m => m.Phone).Take(limit).ToList();
        }

        public int GetTotalCount()
        {
            return _sentMessages.Count;
        }
    }
}
