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
    public class AuditController : ControllerBase
    {
        private readonly AuditService _auditService;

        public AuditController(AuditService auditService)
        {
            _auditService = auditService;
        }

        [HttpGet]
        public ActionResult<ApiResponse> GetLogs(int page = 1, int pageSize = 20, string method = null, string path = null)
        {
            return ApiResponse.Success(_auditService.GetLogs(page, pageSize, method, path));
        }

        [HttpGet]
        public ActionResult<ApiResponse> GetStats()
        {
            return ApiResponse.Success(_auditService.GetStats());
        }

        [HttpPost]
        public ActionResult<ApiResponse> Clear()
        {
            _auditService.Clear();
            return ApiResponse.Success(message: "Logs cleared");
        }
    }
}
