using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class EncodeController : ControllerBase
    {
        private readonly EncodeService _encodeService;

        public EncodeController(EncodeService encodeService)
        {
            _encodeService = encodeService;
        }

        [HttpPost]
        public ActionResult<ApiResponse> Base64Encode([FromBody] InputRequest req)
        {
            return ApiResponse.Success(new { result = _encodeService.Base64Encode(req.Input) });
        }

        [HttpPost]
        public ActionResult<ApiResponse> Base64Decode([FromBody] InputRequest req)
        {
            return ApiResponse.Success(new { result = _encodeService.Base64Decode(req.Input) });
        }

        [HttpPost]
        public ActionResult<ApiResponse> UrlEncode([FromBody] InputRequest req)
        {
            return ApiResponse.Success(new { result = _encodeService.UrlEncode(req.Input) });
        }

        [HttpPost]
        public ActionResult<ApiResponse> UrlDecode([FromBody] InputRequest req)
        {
            return ApiResponse.Success(new { result = _encodeService.UrlDecode(req.Input) });
        }

        [HttpPost]
        public ActionResult<ApiResponse> HtmlEncode([FromBody] InputRequest req)
        {
            return ApiResponse.Success(new { result = _encodeService.HtmlEncode(req.Input) });
        }

        [HttpPost]
        public ActionResult<ApiResponse> HtmlDecode([FromBody] InputRequest req)
        {
            return ApiResponse.Success(new { result = _encodeService.HtmlDecode(req.Input) });
        }

        [HttpPost]
        public ActionResult<ApiResponse> UnicodeEncode([FromBody] InputRequest req)
        {
            return ApiResponse.Success(new { result = _encodeService.UnicodeEncode(req.Input) });
        }

        [HttpPost]
        public ActionResult<ApiResponse> UnicodeDecode([FromBody] InputRequest req)
        {
            return ApiResponse.Success(new { result = _encodeService.UnicodeDecode(req.Input) });
        }

        [HttpPost]
        public ActionResult<ApiResponse> HexEncode([FromBody] InputRequest req)
        {
            return ApiResponse.Success(new { result = _encodeService.HexEncode(req.Input) });
        }

        [HttpPost]
        public ActionResult<ApiResponse> HexDecode([FromBody] InputRequest req)
        {
            return ApiResponse.Success(new { result = _encodeService.HexDecode(req.Input) });
        }
    }
}
