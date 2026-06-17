using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class NumberController : ControllerBase
    {
        private readonly NumberService _numberService;

        public NumberController(NumberService numberService)
        {
            _numberService = numberService;
        }

        [HttpGet]
        public ActionResult<ApiResponse> DecToBin(long number)
        {
            return ApiResponse.Success(new { decimal_ = number, binary = _numberService.DecToBin(number) });
        }

        [HttpGet]
        public ActionResult<ApiResponse> DecToOct(long number)
        {
            return ApiResponse.Success(new { decimal_ = number, octal = _numberService.DecToOct(number) });
        }

        [HttpGet]
        public ActionResult<ApiResponse> DecToHex(long number)
        {
            return ApiResponse.Success(new { decimal_ = number, hexadecimal = _numberService.DecToHex(number) });
        }

        [HttpGet]
        public ActionResult<ApiResponse> BinToDec(string number)
        {
            return ApiResponse.Success(new { binary = number, decimal_ = _numberService.BinToDec(number) });
        }

        [HttpGet]
        public ActionResult<ApiResponse> OctToDec(string number)
        {
            return ApiResponse.Success(new { octal = number, decimal_ = _numberService.OctToDec(number) });
        }

        [HttpGet]
        public ActionResult<ApiResponse> HexToDec(string number)
        {
            return ApiResponse.Success(new { hexadecimal = number, decimal_ = _numberService.HexToDec(number) });
        }

        [HttpGet]
        public ActionResult<ApiResponse> Roman(int number)
        {
            return ApiResponse.Success(_numberService.RomanConvert(number));
        }

        [HttpGet]
        public ActionResult<ApiResponse> RomanToArabic(string roman)
        {
            return ApiResponse.Success(new { roman, arabic = _numberService.ToArabic(roman) });
        }

        [HttpGet]
        public ActionResult<ApiResponse> Math(string op, double a, double b = 0)
        {
            return ApiResponse.Success(_numberService.MathOperation(op, a, b));
        }

        [HttpPost]
        public ActionResult<ApiResponse> Statistics([FromBody] List<double> numbers)
        {
            return ApiResponse.Success(_numberService.Statistics(numbers));
        }

        [HttpGet]
        public ActionResult<ApiResponse> NumberToWords(long number)
        {
            return ApiResponse.Success(new { number, words = _numberService.NumberToWords(number) });
        }
    }
}
