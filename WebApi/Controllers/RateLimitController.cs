using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class RateLimitController : ControllerBase
    {
        private readonly RateLimitService _rateLimitService;

        public RateLimitController(RateLimitService rateLimitService)
        {
            _rateLimitService = rateLimitService;
        }

        [HttpGet]
        public ActionResult<ApiResponse> Check(string key, int limit = 100, int windowSeconds = 60)
        {
            var result = _rateLimitService.Check(key, limit, windowSeconds);
            return ApiResponse.Success(result);
        }

        [HttpPost]
        public ActionResult<ApiResponse> Reset(string key)
        {
            _rateLimitService.Reset(key);
            return ApiResponse.Success(message: $"Key '{key}' reset");
        }

        [HttpPost]
        public ActionResult<ApiResponse> ResetAll()
        {
            _rateLimitService.ResetAll();
            return ApiResponse.Success(message: "All keys reset");
        }

        [HttpGet]
        public ActionResult<ApiResponse> AllKeys()
        {
            return ApiResponse.Success(_rateLimitService.GetAllKeys());
        }
    }
}
