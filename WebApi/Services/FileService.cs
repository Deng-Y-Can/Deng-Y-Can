using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using WebApi.Models;

namespace WebApi.Services
{
    public class FileService
    {
        private readonly string _uploadRoot;
        private const long MaxFileSize = 100 * 1024 * 1024;
        private static readonly string[] AllowedImageTypes = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
        private static readonly string[] AllowedDocTypes = { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".txt", ".csv" };

        public FileService(IWebHostEnvironment env)
        {
            _uploadRoot = Path.Combine(env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot"), "uploads");
            if (!Directory.Exists(_uploadRoot))
                Directory.CreateDirectory(_uploadRoot);
        }

        public async Task<Dictionary<string, object>> UploadAsync(IFormFile file, string subDir = null)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("No file uploaded");

            if (file.Length > MaxFileSize)
                throw new ArgumentException($"File size exceeds limit ({MaxFileSize / 1024 / 1024}MB)");

            var saveDir = string.IsNullOrEmpty(subDir) ? _uploadRoot : Path.Combine(_uploadRoot, subDir);
            if (!Directory.Exists(saveDir))
                Directory.CreateDirectory(saveDir);

            var ext = Path.GetExtension(file.FileName).ToLower();
            var newFileName = $"{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid():N}{ext}";
            var filePath = Path.Combine(saveDir, newFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return new Dictionary<string, object>
            {
                ["originalName"] = file.FileName,
                ["fileName"] = newFileName,
                ["size"] = file.Length,
                ["extension"] = ext,
                ["url"] = $"/uploads/{(string.IsNullOrEmpty(subDir) ? "" : subDir + "/")}{newFileName}",
                ["createdAt"] = DateTime.Now
            };
        }

        public async Task<List<Dictionary<string, object>>> BatchUploadAsync(IFormFileCollection files, string subDir = null)
        {
            var results = new List<Dictionary<string, object>>();
            foreach (var file in files)
            {
                try
                {
                    var result = await UploadAsync(file, subDir);
                    results.Add(result);
                }
                catch (Exception ex)
                {
                    results.Add(new Dictionary<string, object>
                    {
                        ["originalName"] = file.FileName,
                        ["error"] = ex.Message
                    });
                }
            }
            return results;
        }

        public Stream Download(string fileName, out string originalName, out string contentType)
        {
            var filePath = Path.Combine(_uploadRoot, fileName);
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found", fileName);

            originalName = fileName;
            contentType = GetContentType(fileName);
            return File.OpenRead(filePath);
        }

        public bool Delete(string fileName)
        {
            var filePath = Path.Combine(_uploadRoot, fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return true;
            }
            return false;
        }

        public List<Dictionary<string, object>> ListFiles(string subDir = null)
        {
            var dir = string.IsNullOrEmpty(subDir) ? _uploadRoot : Path.Combine(_uploadRoot, subDir);
            if (!Directory.Exists(dir)) return new List<Dictionary<string, object>>();

            return Directory.GetFiles(dir)
                .Select(f => new FileInfo(f))
                .Select(fi => new Dictionary<string, object>
                {
                    ["name"] = fi.Name,
                    ["size"] = fi.Length,
                    ["extension"] = fi.Extension,
                    ["url"] = $"/uploads/{(string.IsNullOrEmpty(subDir) ? "" : subDir + "/")}{fi.Name}",
                    ["createdAt"] = fi.CreationTime,
                    ["modifiedAt"] = fi.LastWriteTime
                })
                .OrderByDescending(f => f["createdAt"])
                .ToList();
        }

        public Dictionary<string, object> GetFileInfo(string fileName)
        {
            var filePath = Path.Combine(_uploadRoot, fileName);
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found", fileName);

            var fi = new FileInfo(filePath);
            return new Dictionary<string, object>
            {
                ["name"] = fi.Name,
                ["size"] = fi.Length,
                ["extension"] = fi.Extension,
                ["url"] = $"/uploads/{fileName}",
                ["createdAt"] = fi.CreationTime,
                ["modifiedAt"] = fi.LastWriteTime
            };
        }

        private string GetContentType(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLower();
            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".webp" => "image/webp",
                ".pdf" => "application/pdf",
                ".doc" or ".docx" => "application/msword",
                ".xls" or ".xlsx" => "application/vnd.ms-excel",
                ".ppt" or ".pptx" => "application/vnd.ms-powerpoint",
                ".txt" => "text/plain",
                ".csv" => "text/csv",
                ".json" => "application/json",
                ".xml" => "application/xml",
                ".zip" => "application/zip",
                _ => "application/octet-stream"
            };
        }
    }
}
