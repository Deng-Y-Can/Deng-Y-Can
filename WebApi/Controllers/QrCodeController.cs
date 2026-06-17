using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class QrCodeController : ControllerBase
    {
        private readonly QrCodeService _qrCodeService;

        public QrCodeController(QrCodeService qrCodeService)
        {
            _qrCodeService = qrCodeService;
        }

        [HttpGet]
        public IActionResult Generate(string text, int size = 10, string foreground = "#000000", string background = "#FFFFFF")
        {
            var svg = _qrCodeService.GenerateSvg(text, size, foreground, background);
            return Content(svg, "image/svg+xml");
        }

        [HttpPost]
        public ActionResult<ApiResponse> GenerateInfo([FromBody] InputRequest req)
        {
            var svg = _qrCodeService.GenerateSvg(req.Input);
            return ApiResponse.Success(new
            {
                format = "svg",
                size = svg.Length,
                svg
            });
        }
    }
}
