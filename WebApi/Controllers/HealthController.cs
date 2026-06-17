using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class HealthController : ControllerBase
    {
        private readonly HealthService _healthService;

        public HealthController(HealthService healthService)
        {
            _healthService = healthService;
        }

        [HttpGet]
        public ActionResult<ApiResponse> Check()
        {
            return ApiResponse.Success(_healthService.GetHealth());
        }

        [HttpGet]
        public ActionResult<ApiResponse> System()
        {
            return ApiResponse.Success(_healthService.GetSystemInfo());
        }

        [HttpGet]
        public ActionResult<ApiResponse> Process()
        {
            return ApiResponse.Success(_healthService.GetProcessInfo());
        }

        [HttpGet]
        public ActionResult<ApiResponse> Memory()
        {
            return ApiResponse.Success(_healthService.GetMemoryInfo());
        }

        [HttpGet]
        public ActionResult<ApiResponse> Disk()
        {
            return ApiResponse.Success(_healthService.GetDiskInfo());
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse>> Ping([FromBody] List<string> hosts)
        {
            return ApiResponse.Success(await _healthService.PingAsync(hosts));
        }
    }
}
