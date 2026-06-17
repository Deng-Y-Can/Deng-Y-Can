using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class JwtController : ControllerBase
    {
        private readonly JwtService _jwtService;

        public JwtController(JwtService jwtService)
        {
            _jwtService = jwtService;
        }

        [HttpPost]
        public ActionResult<ApiResponse> Configure(string secretKey, string issuer = null, string audience = null)
        {
            _jwtService.Configure(secretKey, issuer, audience);
            return ApiResponse.Success(message: "JWT configured");
        }

        [HttpPost]
        public ActionResult<ApiResponse> Generate([FromBody] JwtGenerateRequest req)
        {
            var result = _jwtService.GenerateToken(req.Claims, req.ExpireMinutes);
            return ApiResponse.Success(result);
        }

        [HttpPost]
        public ActionResult<ApiResponse> Validate([FromBody] InputRequest req)
        {
            var result = _jwtService.ValidateToken(req.Input);
            return ApiResponse.Success(result);
        }

        [HttpPost]
        public ActionResult<ApiResponse> Decode([FromBody] InputRequest req)
        {
            var result = _jwtService.DecodeToken(req.Input);
            return ApiResponse.Success(result);
        }
    }

    public class JwtGenerateRequest
    {
        public Dictionary<string, object> Claims { get; set; }
        public int ExpireMinutes { get; set; } = 60;
    }
}
