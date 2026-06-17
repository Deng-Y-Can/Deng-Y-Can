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
    public class CacheController : ControllerBase
    {
        private readonly CacheService _cacheService;

        public CacheController(CacheService cacheService)
        {
            _cacheService = cacheService;
        }

        [HttpGet]
        public ActionResult<ApiResponse> GetAllKeys()
        {
            return ApiResponse.Success(_cacheService.GetAllKeys());
        }

        [HttpGet]
        public ActionResult<ApiResponse> Get(string key)
        {
            var value = _cacheService.Get(key);
            if (value == null) return ApiResponse.Fail("Key not found", 404);
            return ApiResponse.Success(new { key, value });
        }

        [HttpPost]
        public ActionResult<ApiResponse> Set(string key, [FromBody] CacheRequest request)
        {
            var expiry = request.ExpirySeconds > 0
                ? TimeSpan.FromSeconds(request.ExpirySeconds)
                : (TimeSpan?)null;
            _cacheService.Set(key, request.Value, expiry);
            return ApiResponse.Success(message: "Set successfully");
        }

        [HttpPost]
        public ActionResult<ApiResponse> Delete(string key)
        {
            _cacheService.Delete(key);
            return ApiResponse.Success(message: "Deleted");
        }

        [HttpGet]
        public ActionResult<ApiResponse> Exists(string key)
        {
            return ApiResponse.Success(new { key, exists = _cacheService.Exists(key) });
        }

        [HttpGet]
        public ActionResult<ApiResponse> Count()
        {
            return ApiResponse.Success(new { count = _cacheService.Count() });
        }

        [HttpPost]
        public ActionResult<ApiResponse> Clear()
        {
            _cacheService.Clear();
            return ApiResponse.Success(message: "Cache cleared");
        }
    }

    public class CacheRequest
    {
        public object Value { get; set; }
        public int ExpirySeconds { get; set; }
    }
}
