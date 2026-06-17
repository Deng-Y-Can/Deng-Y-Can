using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class CryptoController : ControllerBase
    {
        private readonly CryptoService _cryptoService;

        public CryptoController(CryptoService cryptoService)
        {
            _cryptoService = cryptoService;
        }

        [HttpPost]
        public ActionResult<ApiResponse> Md5([FromBody] InputRequest req)
        {
            return ApiResponse.Success(new { input = req.Input, result = _cryptoService.Md5(req.Input) });
        }

        [HttpPost]
        public ActionResult<ApiResponse> Sha1([FromBody] InputRequest req)
        {
            return ApiResponse.Success(new { input = req.Input, result = _cryptoService.Sha1(req.Input) });
        }

        [HttpPost]
        public ActionResult<ApiResponse> Sha256([FromBody] InputRequest req)
        {
            return ApiResponse.Success(new { input = req.Input, result = _cryptoService.Sha256(req.Input) });
        }

        [HttpPost]
        public ActionResult<ApiResponse> Sha512([FromBody] InputRequest req)
        {
            return ApiResponse.Success(new { input = req.Input, result = _cryptoService.Sha512(req.Input) });
        }

        [HttpPost]
        public ActionResult<ApiResponse> HmacSha256([FromBody] HmacRequest req)
        {
            return ApiResponse.Success(new { result = _cryptoService.HmacSha256(req.Input, req.Key) });
        }

        [HttpPost]
        public ActionResult<ApiResponse> AesEncrypt([FromBody] AesRequest req)
        {
            return ApiResponse.Success(new { result = _cryptoService.AesEncrypt(req.Input, req.Key, req.Iv) });
        }

        [HttpPost]
        public ActionResult<ApiResponse> AesDecrypt([FromBody] AesRequest req)
        {
            return ApiResponse.Success(new { result = _cryptoService.AesDecrypt(req.Input, req.Key, req.Iv) });
        }

        [HttpPost]
        public ActionResult<ApiResponse> RsaKeys()
        {
            return ApiResponse.Success(_cryptoService.GenerateRsaKeys());
        }

        [HttpPost]
        public ActionResult<ApiResponse> RsaEncrypt([FromBody] RsaRequest req)
        {
            return ApiResponse.Success(new { result = _cryptoService.RsaEncrypt(req.Input, req.PublicKey) });
        }

        [HttpPost]
        public ActionResult<ApiResponse> RsaDecrypt([FromBody] RsaDecryptRequest req)
        {
            return ApiResponse.Success(new { result = _cryptoService.RsaDecrypt(req.Input, req.PrivateKey) });
        }
    }

    public class InputRequest
    {
        public string Input { get; set; }
    }

    public class HmacRequest
    {
        public string Input { get; set; }
        public string Key { get; set; }
    }

    public class AesRequest
    {
        public string Input { get; set; }
        public string Key { get; set; }
        public string Iv { get; set; }
    }

    public class RsaRequest
    {
        public string Input { get; set; }
        public string PublicKey { get; set; }
    }

    public class RsaDecryptRequest
    {
        public string Input { get; set; }
        public string PrivateKey { get; set; }
    }
}
