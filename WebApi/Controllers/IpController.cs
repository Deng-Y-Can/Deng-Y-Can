using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class IpController : ControllerBase
    {
        private readonly IpService _ipService;

        public IpController(IpService ipService)
        {
            _ipService = ipService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse>> Info(string ip = null)
        {
            return ApiResponse.Success(await _ipService.GetIpInfo(ip));
        }

        [HttpGet]
        public ActionResult<ApiResponse> MyIp()
        {
            return ApiResponse.Success(new { ip = _ipService.GetClientIp(HttpContext) });
        }

        [HttpGet]
        public ActionResult<ApiResponse> Dns(string hostname)
        {
            return ApiResponse.Success(_ipService.ResolveDns(hostname));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse>> ScanPorts(string host, int startPort = 1, int endPort = 100, int timeoutMs = 200)
        {
            return ApiResponse.Success(await _ipService.ScanPorts(host, startPort, endPort, timeoutMs));
        }
    }
}
