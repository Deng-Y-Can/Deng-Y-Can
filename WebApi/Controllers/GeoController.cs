using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class GeoController : ControllerBase
    {
        private readonly GeoService _geoService;

        public GeoController(GeoService geoService)
        {
            _geoService = geoService;
        }

        [HttpGet]
        public ActionResult<ApiResponse> Distance(double lat1, double lon1, double lat2, double lon2)
        {
            return ApiResponse.Success(_geoService.GetDistance(lat1, lon1, lat2, lon2));
        }

        [HttpGet]
        public ActionResult<ApiResponse> Midpoint(double lat1, double lon1, double lat2, double lon2)
        {
            return ApiResponse.Success(_geoService.Midpoint(lat1, lon1, lat2, lon2));
        }

        [HttpGet]
        public ActionResult<ApiResponse> Circle(double lat, double lon, double radiusKm, int points = 36)
        {
            return ApiResponse.Success(_geoService.GenerateCircle(lat, lon, radiusKm, points));
        }

        [HttpGet]
        public ActionResult<ApiResponse> Bounds(double lat, double lon, double radiusKm)
        {
            return ApiResponse.Success(_geoService.Bounds(lat, lon, radiusKm));
        }

        [HttpGet]
        public ActionResult<ApiResponse> IsInCircle(double pointLat, double pointLon, double centerLat, double centerLon, double radiusKm)
        {
            return ApiResponse.Success(new
            {
                result = _geoService.IsPointInCircle(pointLat, pointLon, centerLat, centerLon, radiusKm)
            });
        }

        [HttpGet]
        public ActionResult<ApiResponse> Bearing(double lat1, double lon1, double lat2, double lon2)
        {
            return ApiResponse.Success(new
            {
                bearing = _geoService.CalculateBearing(lat1, lon1, lat2, lon2)
            });
        }
    }
}
