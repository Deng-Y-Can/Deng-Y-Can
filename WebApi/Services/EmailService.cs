using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using WebApi.Models;

namespace WebApi.Services
{
    public class EmailService
    {
        private SmtpConfig _config;

        public EmailService()
        {
        }

        public void Configure(SmtpConfig config)
        {
            _config = config;
        }

        public SmtpConfig GetConfig()
        {
            return _config;
        }

        public async Task SendAsync(EmailMessage message)
        {
            if (_config == null)
                throw new InvalidOperationException("SMTP not configured. Call /api/notification/smtp/config first.");

            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress(
                _config.FromName ?? _config.Username,
                _config.FromAddress ?? _config.Username));
            mimeMessage.To.Add(MailboxAddress.Parse(message.To));
            mimeMessage.Subject = message.Subject;

            var bodyBuilder = new BodyBuilder();
            if (message.IsHtml)
                bodyBuilder.HtmlBody = message.Body;
            else
                bodyBuilder.TextBody = message.Body;

            if (message.Attachments != null)
            {
                foreach (var filePath in message.Attachments)
                {
                    if (System.IO.File.Exists(filePath))
                        bodyBuilder.Attachments.Add(filePath);
                }
            }

            mimeMessage.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_config.Host, _config.Port, _config.EnableSsl);
            await client.AuthenticateAsync(_config.Username, _config.Password);
            await client.SendAsync(mimeMessage);
            await client.DisconnectAsync(true);
        }

        public async Task BatchSendAsync(List<EmailMessage> messages)
        {
            foreach (var msg in messages)
            {
                await SendAsync(msg);
            }
        }
    }
}
