using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class CrudController : ControllerBase
    {
        private readonly GenericCrudService _crudService;

        public CrudController(GenericCrudService crudService)
        {
            _crudService = crudService;
        }

        [HttpGet]
        public ActionResult<ApiResponse> GetTables()
        {
            return ApiResponse.Success(_crudService.GetTables());
        }

        [HttpGet]
        public ActionResult<ApiResponse> GetCount(string table)
        {
            return ApiResponse.Success(new { table, count = _crudService.Count(table) });
        }

        [HttpGet]
        public ActionResult<ApiResponse> GetAll(string table)
        {
            return ApiResponse.Success(_crudService.GetAll(table));
        }

        [HttpGet]
        public ActionResult<ApiResponse> GetPaged(string table, int page = 1, int pageSize = 20, string keyword = null, string keywordField = null)
        {
            return ApiResponse.Success(_crudService.GetPaged(table, page, pageSize, keyword, keywordField));
        }

        [HttpGet]
        public ActionResult<ApiResponse> GetById(string table, string id)
        {
            var item = _crudService.GetById(table, id);
            if (item == null) return ApiResponse.Fail("Not found", 404);
            return ApiResponse.Success(item);
        }

        [HttpPost]
        public ActionResult<ApiResponse> Create(string table, [FromBody] Dictionary<string, object> data)
        {
            var result = _crudService.Create(table, data);
            return ApiResponse.Success(result, "Created");
        }

        [HttpPost]
        public ActionResult<ApiResponse> BatchCreate(string table, [FromBody] List<Dictionary<string, object>> data)
        {
            _crudService.BatchCreate(table, data);
            return ApiResponse.Success(message: $"Created {data.Count} records");
        }

        [HttpPost]
        public ActionResult<ApiResponse> Update(string table, string id, [FromBody] Dictionary<string, object> data)
        {
            var ok = _crudService.Update(table, id, data);
            if (!ok) return ApiResponse.Fail("Not found", 404);
            return ApiResponse.Success(message: "Updated");
        }

        [HttpPost]
        public ActionResult<ApiResponse> Delete(string table, string id)
        {
            var ok = _crudService.Delete(table, id);
            if (!ok) return ApiResponse.Fail("Not found", 404);
            return ApiResponse.Success(message: "Deleted");
        }

        [HttpPost]
        public ActionResult<ApiResponse> BatchDelete(string table, [FromBody] List<string> ids)
        {
            _crudService.BatchDelete(table, ids);
            return ApiResponse.Success(message: $"Deleted {ids.Count} records");
        }
    }
}
