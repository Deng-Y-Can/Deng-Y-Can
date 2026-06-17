using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Middleware
{
    public class AuditMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AuditService _auditService;

        public AuditMiddleware(RequestDelegate next, AuditService auditService)
        {
            _next = next;
            _auditService = auditService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var sw = Stopwatch.StartNew();
            var request = context.Request;

            string body = null;
            if (request.ContentLength > 0 && request.ContentLength < 10240)
            {
                try
                {
                    request.EnableBuffering();
                    using var reader = new StreamReader(request.Body, leaveOpen: true);
                    body = await reader.ReadToEndAsync();
                    request.Body.Position = 0;
                }
                catch { }
            }

            var originalBodyStream = context.Response.Body;
            var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            try
            {
                await _next(context);
            }
            finally
            {
                sw.Stop();
                var log = new AuditLog
                {
                    Method = request.Method,
                    Path = request.Path,
                    QueryString = request.QueryString.ToString(),
                    Body = body?.Length > 500 ? body.Substring(0, 500) : body,
                    Ip = context.Connection.RemoteIpAddress?.ToString(),
                    StatusCode = context.Response.StatusCode,
                    ElapsedMs = sw.ElapsedMilliseconds
                };
                _auditService.AddLog(log);

                responseBody.Position = 0;
                await responseBody.CopyToAsync(originalBodyStream);
                context.Response.Body = originalBodyStream;
            }
        }
    }
}
