using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class MockController : ControllerBase
    {
        private readonly MockService _mockService;

        public MockController(MockService mockService)
        {
            _mockService = mockService;
        }

        [HttpPost]
        public ActionResult<ApiResponse> Register(string path, string method = "ANY", int statusCode = 200,
            int delayMs = 0, [FromBody] object responseBody = null)
        {
            var endpoint = _mockService.Register(path, method, responseBody, statusCode, delayMs);
            return ApiResponse.Success(endpoint, "Mock registered");
        }

        [HttpGet]
        public ActionResult<ApiResponse> All()
        {
            return ApiResponse.Success(_mockService.GetAll());
        }

        [HttpPost]
        public ActionResult<ApiResponse> Remove(string path)
        {
            var ok = _mockService.Remove(path);
            if (!ok) return ApiResponse.Fail("Not found", 404);
            return ApiResponse.Success(message: "Removed");
        }

        [HttpPost]
        public ActionResult<ApiResponse> Clear()
        {
            _mockService.Clear();
            return ApiResponse.Success(message: "All mocks cleared");
        }

        [HttpGet, HttpPost, HttpPut, HttpDelete, HttpPatch]
        [Route("/mock/{**path}")]
        public async Task<IActionResult> Mock(string path)
        {
            var fullPath = "/mock/" + path;
            var method = Request.Method;
            var endpoint = _mockService.Match(fullPath, method);

            if (endpoint == null)
                return NotFound(new ApiResponse { Code = 404, Message = $"No mock registered for {method} {fullPath}" });

            if (endpoint.DelayMs > 0)
                await Task.Delay(endpoint.DelayMs);

            foreach (var header in endpoint.Headers)
                Response.Headers[header.Key] = header.Value;

            Response.StatusCode = endpoint.StatusCode;
            return Content(
                Newtonsoft.Json.JsonConvert.SerializeObject(endpoint.ResponseBody),
                "application/json");
        }
    }
}
