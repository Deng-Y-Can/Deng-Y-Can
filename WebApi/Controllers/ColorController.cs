using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ColorController : ControllerBase
    {
        private readonly ColorService _colorService;

        public ColorController(ColorService colorService)
        {
            _colorService = colorService;
        }

        [HttpGet]
        public ActionResult<ApiResponse> Parse(string color)
        {
            return ApiResponse.Success(_colorService.Parse(color));
        }

        [HttpGet]
        public ActionResult<ApiResponse> ToHex(string color)
        {
            var hex = _colorService.ToHex(color);
            return ApiResponse.Success(new { hex });
        }

        [HttpGet]
        public ActionResult<ApiResponse> Palette(string color, int count = 5)
        {
            return ApiResponse.Success(_colorService.GeneratePalette(color, count));
        }

        [HttpGet]
        public ActionResult<ApiResponse> Complementary(string color)
        {
            return ApiResponse.Success(_colorService.Complementary(color));
        }

        [HttpGet]
        public ActionResult<ApiResponse> Shades(string color, int count = 5)
        {
            return ApiResponse.Success(_colorService.Shades(color, count));
        }
    }
}
