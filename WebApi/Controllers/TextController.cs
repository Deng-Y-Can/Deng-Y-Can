using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TextController : ControllerBase
    {
        private readonly TextService _textService;

        public TextController(TextService textService)
        {
            _textService = textService;
        }

        [HttpPost]
        public ActionResult<ApiResponse> Analyze([FromBody] InputRequest req)
        {
            return ApiResponse.Success(_textService.Analyze(req.Input));
        }

        [HttpPost]
        public ActionResult<ApiResponse> Reverse([FromBody] InputRequest req)
        {
            return ApiResponse.Success(new { result = _textService.Reverse(req.Input) });
        }

        [HttpPost]
        public ActionResult<ApiResponse> ToUpper([FromBody] InputRequest req)
        {
            return ApiResponse.Success(new { result = _textService.ToUpper(req.Input) });
        }

        [HttpPost]
        public ActionResult<ApiResponse> ToLower([FromBody] InputRequest req)
        {
            return ApiResponse.Success(new { result = _textService.ToLower(req.Input) });
        }

        [HttpPost]
        public ActionResult<ApiResponse> ToTitleCase([FromBody] InputRequest req)
        {
            return ApiResponse.Success(new { result = _textService.ToTitleCase(req.Input) });
        }

        [HttpPost]
        public ActionResult<ApiResponse> ToCamelCase([FromBody] InputRequest req)
        {
            return ApiResponse.Success(new { result = _textService.ToCamelCase(req.Input) });
        }

        [HttpPost]
        public ActionResult<ApiResponse> ToSnakeCase([FromBody] InputRequest req)
        {
            return ApiResponse.Success(new { result = _textService.ToSnakeCase(req.Input) });
        }

        [HttpPost]
        public ActionResult<ApiResponse> ToKebabCase([FromBody] InputRequest req)
        {
            return ApiResponse.Success(new { result = _textService.ToKebabCase(req.Input) });
        }

        [HttpPost]
        public ActionResult<ApiResponse> Truncate(string text, int maxLength, string suffix = "...")
        {
            return ApiResponse.Success(new { result = _textService.Truncate(text, maxLength, suffix) });
        }

        [HttpPost]
        public ActionResult<ApiResponse> Repeat(string text, int count)
        {
            return ApiResponse.Success(new { result = _textService.Repeat(text, count) });
        }

        [HttpPost]
        public ActionResult<ApiResponse> Mask(string text, string pattern = "*")
        {
            return ApiResponse.Success(new { result = _textService.Mask(text, pattern) });
        }

        [HttpPost]
        public ActionResult<ApiResponse> MaskEmail(string email)
        {
            return ApiResponse.Success(new { result = _textService.MaskEmail(email) });
        }

        [HttpPost]
        public ActionResult<ApiResponse> MaskPhone(string phone)
        {
            return ApiResponse.Success(new { result = _textService.MaskPhone(phone) });
        }

        [HttpPost]
        public ActionResult<ApiResponse> ExtractEmails([FromBody] InputRequest req)
        {
            return ApiResponse.Success(_textService.ExtractEmails(req.Input));
        }

        [HttpPost]
        public ActionResult<ApiResponse> ExtractUrls([FromBody] InputRequest req)
        {
            return ApiResponse.Success(_textService.ExtractUrls(req.Input));
        }

        [HttpPost]
        public ActionResult<ApiResponse> Diff([FromBody] DiffRequest req)
        {
            return ApiResponse.Success(new { result = _textService.Diff(req.Text1, req.Text2) });
        }

        [HttpPost]
        public ActionResult<ApiResponse> SortLines([FromBody] InputRequest req, bool descending = false)
        {
            return ApiResponse.Success(new { result = _textService.SortLines(req.Input, descending) });
        }

        [HttpPost]
        public ActionResult<ApiResponse> RemoveDuplicates([FromBody] InputRequest req)
        {
            return ApiResponse.Success(new { result = _textService.RemoveDuplicates(req.Input) });
        }
    }

    public class DiffRequest
    {
        public string Text1 { get; set; }
        public string Text2 { get; set; }
    }
}
