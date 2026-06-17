using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using CsvHelper;
using CsvHelper.Configuration;
using WebApi.Models;

namespace WebApi.Services
{
    public class ExportService
    {
        private readonly string _exportRoot;

        public ExportService()
        {
            _exportRoot = Path.Combine(AppContext.BaseDirectory, "exports");
            if (!Directory.Exists(_exportRoot))
                Directory.CreateDirectory(_exportRoot);
        }

        public byte[] ExportToCsv(string title, List<Dictionary<string, object>> data)
        {
            var memoryStream = new MemoryStream();
            var writer = new StreamWriter(memoryStream);
            var csv = new CsvWriter(writer, new Configuration { HasHeaderRecord = true, Encoding = System.Text.Encoding.UTF8 });

            if (data == null || !data.Any())
            {
                writer.Flush();
                return memoryStream.ToArray();
            }

            var headers = data.First().Keys.ToList();
            foreach (var header in headers)
            {
                csv.WriteField(header);
            }
            csv.NextRecord();

            foreach (var row in data)
            {
                foreach (var header in headers)
                {
                    csv.WriteField(row.ContainsKey(header) ? row[header]?.ToString() : "");
                }
                csv.NextRecord();
            }

            writer.Flush();
            return memoryStream.ToArray();
        }

        public byte[] ExportToExcel(string title, List<Dictionary<string, object>> data)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(title ?? "Sheet1");

            if (data == null || !data.Any())
            {
                var memoryStreamEmpty = new MemoryStream();
                workbook.SaveAs(memoryStreamEmpty);
                return memoryStreamEmpty.ToArray();
            }

            var headers = data.First().Keys.ToList();

            // Header style
            for (int i = 0; i < headers.Count; i++)
            {
                var cell = worksheet.Cell(1, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.LightGray;
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }

            // Data
            for (int r = 0; r < data.Count; r++)
            {
                for (int c = 0; c < headers.Count; c++)
                {
                    var val = data[r].ContainsKey(headers[c]) ? data[r][headers[c]] : null;
                    var cell = worksheet.Cell(r + 2, c + 1);
                    cell.Value = val?.ToString() ?? "";
                    cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                }
            }

            worksheet.Columns().AdjustToContents();

            var memoryStream = new MemoryStream();
            workbook.SaveAs(memoryStream);
            return memoryStream.ToArray();
        }

        public Dictionary<string, object> SaveExportFile(byte[] data, string fileName)
        {
            var filePath = Path.Combine(_exportRoot, fileName);
            File.WriteAllBytes(filePath, data);
            return new Dictionary<string, object>
            {
                ["fileName"] = fileName,
                ["size"] = data.Length,
                ["path"] = filePath,
                ["createdAt"] = DateTime.Now
            };
        }

        public Stream GetExportFile(string fileName)
        {
            var filePath = Path.Combine(_exportRoot, fileName);
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Export file not found", fileName);
            return File.OpenRead(filePath);
        }

        public List<Dictionary<string, object>> ListExportFiles()
        {
            if (!Directory.Exists(_exportRoot))
                return new List<Dictionary<string, object>>();

            return Directory.GetFiles(_exportRoot)
                .Select(f => new FileInfo(f))
                .Select(fi => new Dictionary<string, object>
                {
                    ["fileName"] = fi.Name,
                    ["size"] = fi.Length,
                    ["createdAt"] = fi.CreationTime,
                    ["extension"] = fi.Extension
                })
                .OrderByDescending(f => f["createdAt"])
                .ToList();
        }

        public bool DeleteExportFile(string fileName)
        {
            var filePath = Path.Combine(_exportRoot, fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return true;
            }
            return false;
        }
    }
}
