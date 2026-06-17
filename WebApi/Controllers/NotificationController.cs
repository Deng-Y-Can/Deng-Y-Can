using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class NotificationController : ControllerBase
    {
        private readonly EmailService _emailService;
        private readonly SmsService _smsService;

        public NotificationController(EmailService emailService, SmsService smsService)
        {
            _emailService = emailService;
            _smsService = smsService;
        }

        [HttpPost]
        public ActionResult<ApiResponse> ConfigureSmtp([FromBody] SmtpConfig config)
        {
            _emailService.Configure(config);
            return ApiResponse.Success(message: "SMTP configured");
        }

        [HttpGet]
        public ActionResult<ApiResponse> GetSmtpConfig()
        {
            var config = _emailService.GetConfig();
            if (config == null) return ApiResponse.Fail("SMTP not configured");
            config.Password = "******";
            return ApiResponse.Success(config);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse>> SendEmail([FromBody] EmailMessage message)
        {
            try
            {
                await _emailService.SendAsync(message);
                return ApiResponse.Success(message: "Email sent");
            }
            catch (Exception ex)
            {
                return ApiResponse.Fail(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse>> BatchSendEmail([FromBody] List<EmailMessage> messages)
        {
            try
            {
                await _emailService.BatchSendAsync(messages);
                return ApiResponse.Success(message: $"Sent {messages.Count} emails");
            }
            catch (Exception ex)
            {
                return ApiResponse.Fail(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse>> SendSms([FromBody] SmsMessage message)
        {
            try
            {
                await _smsService.SendAsync(message);
                return ApiResponse.Success(message: "SMS sent");
            }
            catch (Exception ex)
            {
                return ApiResponse.Fail(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult<ApiResponse> SmsHistory(int limit = 100)
        {
            return ApiResponse.Success(_smsService.GetHistory(limit));
        }

        [HttpGet]
        public ActionResult<ApiResponse> SmsStats()
        {
            return ApiResponse.Success(new { totalSent = _smsService.GetTotalCount() });
        }
    }
}
