using System;
using System.Collections.Generic;

namespace WebApi.Models
{
    public class AuditLog
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string Method { get; set; }
        public string Path { get; set; }
        public string QueryString { get; set; }
        public string Body { get; set; }
        public string Ip { get; set; }
        public int StatusCode { get; set; }
        public long ElapsedMs { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public class DictItem
    {
        public string DictCode { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int SortOrder { get; set; }
        public bool Enabled { get; set; } = true;
        public string Remark { get; set; }
    }

    public class DictGroup
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public List<DictItem> Items { get; set; } = new List<DictItem>();
    }

    public class SmtpConfig
    {
        public string Host { get; set; }
        public int Port { get; set; } = 587;
        public bool EnableSsl { get; set; } = true;
        public string Username { get; set; }
        public string Password { get; set; }
        public string FromName { get; set; }
        public string FromAddress { get; set; }
    }

    public class EmailMessage
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsHtml { get; set; } = true;
        public List<string> Attachments { get; set; }
    }

    public class SmsMessage
    {
        public string Phone { get; set; }
        public string Content { get; set; }
    }

    public class JobInfo
    {
        public string JobName { get; set; }
        public string Group { get; set; }
        public string Description { get; set; }
        public bool IsRunning { get; set; }
        public DateTime? NextFireTime { get; set; }
        public DateTime? PreviousFireTime { get; set; }
    }
}
