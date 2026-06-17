using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class FileController : ControllerBase
    {
        private readonly FileService _fileService;

        public FileController(FileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse>> Upload(IFormFile file, string subDir = null)
        {
            try
            {
                var result = await _fileService.UploadAsync(file, subDir);
                return ApiResponse.Success(result, "Uploaded");
            }
            catch (Exception ex)
            {
                return ApiResponse.Fail(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse>> BatchUpload(IFormFileCollection files, string subDir = null)
        {
            var results = await _fileService.BatchUploadAsync(files, subDir);
            return ApiResponse.Success(results);
        }

        [HttpGet]
        public IActionResult Download(string fileName)
        {
            try
            {
                var stream = _fileService.Download(fileName, out var originalName, out var contentType);
                return File(stream, contentType, originalName);
            }
            catch (FileNotFoundException ex)
            {
                return BadRequest(ApiResponse.Fail(ex.Message, 404));
            }
        }

        [HttpGet]
        public ActionResult<ApiResponse> List(string subDir = null)
        {
            return ApiResponse.Success(_fileService.ListFiles(subDir));
        }

        [HttpGet]
        public ActionResult<ApiResponse> Info(string fileName)
        {
            try
            {
                return ApiResponse.Success(_fileService.GetFileInfo(fileName));
            }
            catch (FileNotFoundException ex)
            {
                return ApiResponse.Fail(ex.Message, 404);
            }
        }

        [HttpPost]
        public ActionResult<ApiResponse> Delete(string fileName)
        {
            var ok = _fileService.Delete(fileName);
            if (!ok) return ApiResponse.Fail("File not found", 404);
            return ApiResponse.Success(message: "Deleted");
        }
    }
}
