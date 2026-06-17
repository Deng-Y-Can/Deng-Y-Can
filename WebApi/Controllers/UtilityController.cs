using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UtilityController : ControllerBase
    {
        private readonly UtilityService _utilityService;

        public UtilityController(UtilityService utilityService)
        {
            _utilityService = utilityService;
        }

        [HttpGet]
        public ActionResult<ApiResponse> Guid(bool noDash = false)
        {
            return ApiResponse.Success(new { guid = _utilityService.NewGuid(noDash) });
        }

        [HttpGet]
        public ActionResult<ApiResponse> Id(int length = 16)
        {
            return ApiResponse.Success(new { id = _utilityService.NewId(length) });
        }

        [HttpGet]
        public ActionResult<ApiResponse> Timestamp(bool ms = false)
        {
            return ApiResponse.Success(ms
                ? (object)_utilityService.TimestampMs()
                : _utilityService.Timestamp());
        }

        [HttpPost]
        public ActionResult<ApiResponse> TimestampToDate(long timestamp, bool isMs = false)
        {
            return ApiResponse.Success(new { datetime = _utilityService.TimestampToDateTime(timestamp, isMs) });
        }

        [HttpPost]
        public ActionResult<ApiResponse> DateToTimestamp(string dateTime)
        {
            return ApiResponse.Success(new { timestamp = _utilityService.DateTimeToTimestamp(dateTime) });
        }

        [HttpGet]
        public ActionResult<ApiResponse> TimeInfo()
        {
            return ApiResponse.Success(_utilityService.GetTimeInfo());
        }

        [HttpPost]
        public ActionResult<ApiResponse> PrettyJson([FromBody] InputRequest req)
        {
            return ApiResponse.Success(new { result = _utilityService.PrettyJson(req.Input) });
        }

        [HttpPost]
        public ActionResult<ApiResponse> MinifyJson([FromBody] InputRequest req)
        {
            return ApiResponse.Success(new { result = _utilityService.MinifyJson(req.Input) });
        }

        [HttpPost]
        public ActionResult<ApiResponse> JsonToXml([FromBody] InputRequest req)
        {
            return ApiResponse.Success(new { result = _utilityService.JsonToXml(req.Input) });
        }

        [HttpPost]
        public ActionResult<ApiResponse> XmlToJson([FromBody] InputRequest req)
        {
            return ApiResponse.Success(new { result = _utilityService.XmlToJson(req.Input) });
        }

        [HttpPost]
        public ActionResult<ApiResponse> RegexTest([FromBody] RegexRequest req)
        {
            return ApiResponse.Success(new { result = _utilityService.RegexTest(req.Pattern, req.Input) });
        }

        [HttpPost]
        public ActionResult<ApiResponse> RegexReplace([FromBody] RegexReplaceRequest req)
        {
            return ApiResponse.Success(new { result = _utilityService.RegexReplace(req.Pattern, req.Input, req.Replacement) });
        }
    }

    public class RegexRequest
    {
        public string Pattern { get; set; }
        public string Input { get; set; }
    }

    public class RegexReplaceRequest
    {
        public string Pattern { get; set; }
        public string Input { get; set; }
        public string Replacement { get; set; }
    }
}
