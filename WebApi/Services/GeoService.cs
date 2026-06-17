using System;
using System.Collections.Generic;

namespace WebApi.Services
{
    public class GeoService
    {
        private const double EarthRadiusKm = 6371.0;
        private const double EarthRadiusMiles = 3958.8;

        public double HaversineDistance(double lat1, double lon1, double lat2, double lon2, bool miles = false)
        {
            double R = miles ? EarthRadiusMiles : EarthRadiusKm;
            double dLat = ToRad(lat2 - lat1);
            double dLon = ToRad(lon2 - lon1);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return Math.Round(R * c, 2);
        }

        public Dictionary<string, object> GetDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var km = HaversineDistance(lat1, lon1, lat2, lon2, false);
            var miles = HaversineDistance(lat1, lon1, lat2, lon2, true);
            var bearing = CalculateBearing(lat1, lon1, lat2, lon2);

            return new Dictionary<string, object>
            {
                ["from"] = new { lat = lat1, lon = lon1 },
                ["to"] = new { lat = lat2, lon = lon2 },
                ["distanceKm"] = km,
                ["distanceMiles"] = miles,
                ["distanceMeters"] = Math.Round(km * 1000, 0),
                ["bearing"] = bearing
            };
        }

        public double CalculateBearing(double lat1, double lon1, double lat2, double lon2)
        {
            double dLon = ToRad(lon2 - lon1);
            double y = Math.Sin(dLon) * Math.Cos(ToRad(lat2));
            double x = Math.Cos(ToRad(lat1)) * Math.Sin(ToRad(lat2)) -
                       Math.Sin(ToRad(lat1)) * Math.Cos(ToRad(lat2)) * Math.Cos(dLon);
            double bearing = Math.Atan2(y, x);
            return Math.Round((ToDeg(bearing) + 360) % 360, 1);
        }

        public List<Dictionary<string, object>> GenerateCircle(double lat, double lon, double radiusKm, int points = 36)
        {
            var circlePoints = new List<Dictionary<string, object>>();
            for (int i = 0; i < points; i++)
            {
                double angle = 2 * Math.PI * i / points;
                double latRad = ToRad(lat);
                double lonRad = ToRad(lon);
                double angularDistance = radiusKm / EarthRadiusKm;

                double newLat = Math.Asin(Math.Sin(latRad) * Math.Cos(angularDistance) +
                    Math.Cos(latRad) * Math.Sin(angularDistance) * Math.Cos(angle));
                double newLon = lonRad + Math.Atan2(
                    Math.Sin(angle) * Math.Sin(angularDistance) * Math.Cos(latRad),
                    Math.Cos(angularDistance) - Math.Sin(latRad) * Math.Sin(newLat));

                circlePoints.Add(new Dictionary<string, object>
                {
                    ["lat"] = Math.Round(ToDeg(newLat), 6),
                    ["lon"] = Math.Round(ToDeg(newLon), 6),
                    ["index"] = i
                });
            }
            return circlePoints;
        }

        public Dictionary<string, object> Midpoint(double lat1, double lon1, double lat2, double lon2)
        {
            double lat1Rad = ToRad(lat1), lon1Rad = ToRad(lon1);
            double lat2Rad = ToRad(lat2), lon2Rad = ToRad(lon2);

            double bx = Math.Cos(lat2Rad) * Math.Cos(lon2Rad - lon1Rad);
            double by = Math.Cos(lat2Rad) * Math.Sin(lon2Rad - lon1Rad);
            double lat = Math.Atan2(Math.Sin(lat1Rad) + Math.Sin(lat2Rad),
                Math.Sqrt((Math.Cos(lat1Rad) + bx) * (Math.Cos(lat1Rad) + bx) + by * by));
            double lon = lon1Rad + Math.Atan2(by, Math.Cos(lat1Rad) + bx);

            return new Dictionary<string, object>
            {
                ["midpoint"] = new { lat = Math.Round(ToDeg(lat), 6), lon = Math.Round(ToDeg(lon), 6) }
            };
        }

        public bool IsPointInCircle(double pointLat, double pointLon, double centerLat, double centerLon, double radiusKm)
        {
            return HaversineDistance(pointLat, pointLon, centerLat, centerLon) <= radiusKm;
        }

        public Dictionary<string, object> Bounds(double lat, double lon, double radiusKm)
        {
            double latOffset = radiusKm / 111.0;
            double lonOffset = radiusKm / (111.0 * Math.Cos(ToRad(lat)));
            return new Dictionary<string, object>
            {
                ["north"] = Math.Round(lat + latOffset, 6),
                ["south"] = Math.Round(lat - latOffset, 6),
                ["east"] = Math.Round(lon + lonOffset, 6),
                ["west"] = Math.Round(lon - lonOffset, 6),
                ["center"] = new { lat, lon },
                ["radiusKm"] = radiusKm
            };
        }

        private double ToRad(double deg) => deg * Math.PI / 180.0;
        private double ToDeg(double rad) => rad * 180.0 / Math.PI;
    }
}
