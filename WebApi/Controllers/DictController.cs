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
    public class DictController : ControllerBase
    {
        private readonly DictService _dictService;

        public DictController(DictService dictService)
        {
            _dictService = dictService;
        }

        [HttpGet]
        public ActionResult<ApiResponse> GetAll()
        {
            return ApiResponse.Success(_dictService.GetAll());
        }

        [HttpGet]
        public ActionResult<ApiResponse> GetByCode(string code)
        {
            var group = _dictService.GetByCode(code);
            if (group == null) return ApiResponse.Fail("Dictionary not found", 404);
            return ApiResponse.Success(group);
        }

        [HttpGet]
        public ActionResult<ApiResponse> GetItems(string dictCode)
        {
            return ApiResponse.Success(_dictService.GetItems(dictCode));
        }

        [HttpGet]
        public ActionResult<ApiResponse> GetAllItems(string dictCode)
        {
            return ApiResponse.Success(_dictService.GetAllItems(dictCode));
        }

        [HttpGet]
        public ActionResult<ApiResponse> GetItemName(string dictCode, string itemCode)
        {
            var name = _dictService.GetItemName(dictCode, itemCode);
            return ApiResponse.Success(new { dictCode, itemCode, itemName = name });
        }

        [HttpPost]
        public ActionResult<ApiResponse> AddGroup(string code, string name)
        {
            _dictService.AddGroup(code, name);
            return ApiResponse.Success(message: "Group created");
        }

        [HttpPost]
        public ActionResult<ApiResponse> DeleteGroup(string code)
        {
            var ok = _dictService.DeleteGroup(code);
            if (!ok) return ApiResponse.Fail("Group not found", 404);
            return ApiResponse.Success(message: "Group deleted");
        }

        [HttpPost]
        public ActionResult<ApiResponse> AddItem(string dictCode, [FromBody] DictItem item)
        {
            var ok = _dictService.AddItem(dictCode, item);
            if (!ok) return ApiResponse.Fail("Dictionary not found", 404);
            return ApiResponse.Success(message: "Item added");
        }

        [HttpPost]
        public ActionResult<ApiResponse> UpdateItem(string dictCode, string itemCode, [FromBody] DictItem item)
        {
            var ok = _dictService.UpdateItem(dictCode, itemCode, item);
            if (!ok) return ApiResponse.Fail("Item not found", 404);
            return ApiResponse.Success(message: "Item updated");
        }

        [HttpPost]
        public ActionResult<ApiResponse> DeleteItem(string dictCode, string itemCode)
        {
            var ok = _dictService.DeleteItem(dictCode, itemCode);
            if (!ok) return ApiResponse.Fail("Item not found", 404);
            return ApiResponse.Success(message: "Item deleted");
        }
    }
}
