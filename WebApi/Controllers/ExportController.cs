using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ExportController : ControllerBase
    {
        private readonly ExportService _exportService;
        private readonly GenericCrudService _crudService;

        public ExportController(ExportService exportService, GenericCrudService crudService)
        {
            _exportService = exportService;
            _crudService = crudService;
        }

        [HttpPost]
        public ActionResult<ApiResponse> ExportCsv(string title, string table, [FromBody] List<Dictionary<string, object>> data = null)
        {
            try
            {
                var exportData = data ?? _crudService.GetAll(table);
                var bytes = _exportService.ExportToCsv(title ?? table, exportData);
                var fileName = $"{title ?? table}_{DateTime.Now:yyyyMMddHHmmss}.csv";
                return File(bytes, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                return ApiResponse.Fail(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult<ApiResponse> ExportExcel(string title, string table, [FromBody] List<Dictionary<string, object>> data = null)
        {
            try
            {
                var exportData = data ?? _crudService.GetAll(table);
                var bytes = _exportService.ExportToExcel(title ?? table, exportData);
                var fileName = $"{title ?? table}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                return ApiResponse.Fail(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult<ApiResponse> ListExports()
        {
            return ApiResponse.Success(_exportService.ListExportFiles());
        }

        [HttpGet]
        public IActionResult DownloadExport(string fileName)
        {
            try
            {
                var stream = _exportService.GetExportFile(fileName);
                return File(stream, "application/octet-stream", fileName);
            }
            catch (FileNotFoundException ex)
            {
                return BadRequest(ApiResponse.Fail(ex.Message, 404));
            }
        }

        [HttpPost]
        public ActionResult<ApiResponse> DeleteExport(string fileName)
        {
            var ok = _exportService.DeleteExportFile(fileName);
            if (!ok) return ApiResponse.Fail("File not found", 404);
            return ApiResponse.Success(message: "Deleted");
        }
    }
}
