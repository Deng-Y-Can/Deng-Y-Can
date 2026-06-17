using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ProxyController : ControllerBase
    {
        private readonly ProxyService _proxyService;

        public ProxyController(ProxyService proxyService)
        {
            _proxyService = proxyService;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse>> Forward([FromBody] ProxyRequest req)
        {
            var result = await _proxyService.ForwardAsync(
                req.Method ?? "GET", req.Url, req.Headers, req.Body, req.TimeoutSeconds);
            return ApiResponse.Success(result);
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse>> Get(string url)
        {
            var result = await _proxyService.GetAsync(url);
            return ApiResponse.Success(result);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse>> Post(string url, [FromBody] string body)
        {
            var result = await _proxyService.PostAsync(url, body);
            return ApiResponse.Success(result);
        }
    }

    public class ProxyRequest
    {
        public string Method { get; set; }
        public string Url { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public string Body { get; set; }
        public int TimeoutSeconds { get; set; } = 30;
    }
}
