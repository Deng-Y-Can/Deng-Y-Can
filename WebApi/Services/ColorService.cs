using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace WebApi.Services
{
    public class ColorService
    {
        public Dictionary<string, string> Parse(string color)
        {
            var hex = ToHex(color);
            if (hex == null) return new Dictionary<string, string> { ["error"] = "Invalid color" };

            var r = Convert.ToInt32(hex.Substring(1, 2), 16);
            var g = Convert.ToInt32(hex.Substring(3, 2), 16);
            var b = Convert.ToInt32(hex.Substring(5, 2), 16);
            var hsl = RgbToHsl(r, g, b);

            return new Dictionary<string, string>
            {
                ["hex"] = hex,
                ["rgb"] = $"rgb({r}, {g}, {b})",
                ["rgba"] = $"rgba({r}, {g}, {b}, 1.0)",
                ["hsl"] = $"hsl({hsl.h}, {hsl.s}%, {hsl.l}%)",
                ["hsla"] = $"hsla({hsl.h}, {hsl.s}%, {hsl.l}%, 1.0)",
                ["r"] = r.ToString(),
                ["g"] = g.ToString(),
                ["b"] = b.ToString(),
                ["h"] = hsl.h.ToString(),
                ["s"] = hsl.s.ToString(),
                ["l"] = hsl.l.ToString()
            };
        }

        public string ToHex(string color)
        {
            if (color.StartsWith("#") && (color.Length == 4 || color.Length == 7))
            {
                if (color.Length == 4)
                    return $"#{color[1]}{color[1]}{color[2]}{color[2]}{color[3]}{color[3]}";
                return color;
            }

            var match = Regex.Match(color, @"rgb\s*\(\s*(\d+)\s*,\s*(\d+)\s*,\s*(\d+)\s*\)");
            if (match.Success)
            {
                var r = int.Parse(match.Groups[1].Value);
                var g = int.Parse(match.Groups[2].Value);
                var b = int.Parse(match.Groups[3].Value);
                return $"#{r:X2}{g:X2}{b:X2}";
            }

            return null;
        }

        public List<Dictionary<string, string>> GeneratePalette(string baseColor, int count = 5)
        {
            var hex = ToHex(baseColor);
            if (hex == null) return new List<Dictionary<string, string>>();

            var r = Convert.ToInt32(hex.Substring(1, 2), 16);
            var g = Convert.ToInt32(hex.Substring(3, 2), 16);
            var b = Convert.ToInt32(hex.Substring(5, 2), 16);
            var hsl = RgbToHsl(r, g, b);

            var palette = new List<Dictionary<string, string>>();
            for (int i = 0; i < count; i++)
            {
                var newH = (hsl.h + (360.0 / count * i)) % 360;
                var rgb = HslToRgb((int)newH, hsl.s, hsl.l);
                var newHex = $"#{rgb.r:X2}{rgb.g:X2}{rgb.b:X2}";
                palette.Add(new Dictionary<string, string>
                {
                    ["hex"] = newHex,
                    ["rgb"] = $"rgb({rgb.r}, {rgb.g}, {rgb.b})"
                });
            }
            return palette;
        }

        public List<Dictionary<string, string>> Complementary(string color)
        {
            var hex = ToHex(color);
            if (hex == null) return new List<Dictionary<string, string>>();

            var r = Convert.ToInt32(hex.Substring(1, 2), 16);
            var g = Convert.ToInt32(hex.Substring(3, 2), 16);
            var b = Convert.ToInt32(hex.Substring(5, 2), 16);
            var hsl = RgbToHsl(r, g, b);

            var result = new List<Dictionary<string, string>>();
            var complementary = HslToRgb((hsl.h + 180) % 360, hsl.s, hsl.l);
            result.Add(new Dictionary<string, string> { ["hex"] = hex, ["name"] = "original" });
            result.Add(new Dictionary<string, string>
            {
                ["hex"] = $"#{complementary.r:X2}{complementary.g:X2}{complementary.b:X2}",
                ["name"] = "complementary"
            });
            return result;
        }

        public List<Dictionary<string, string>> Shades(string color, int count = 5)
        {
            var hex = ToHex(color);
            if (hex == null) return new List<Dictionary<string, string>>();

            var r = Convert.ToInt32(hex.Substring(1, 2), 16);
            var g = Convert.ToInt32(hex.Substring(3, 2), 16);
            var b = Convert.ToInt32(hex.Substring(5, 2), 16);
            var hsl = RgbToHsl(r, g, b);

            var shades = new List<Dictionary<string, string>>();
            for (int i = 0; i < count; i++)
            {
                var newL = Math.Max(0, Math.Min(100, hsl.l - (i * 100 / count)));
                var rgb = HslToRgb(hsl.h, hsl.s, (int)newL);
                var newHex = $"#{rgb.r:X2}{rgb.g:X2}{rgb.b:X2}";
                shades.Add(new Dictionary<string, string>
                {
                    ["hex"] = newHex,
                    ["lightness"] = ((int)newL).ToString()
                });
            }
            return shades;
        }

        private (int h, int s, int l) RgbToHsl(int r, int g, int b)
        {
            double rNorm = r / 255.0, gNorm = g / 255.0, bNorm = b / 255.0;
            double max = Math.Max(rNorm, Math.Max(gNorm, bNorm));
            double min = Math.Min(rNorm, Math.Min(gNorm, bNorm));
            double h = 0, s = 0, l = (max + min) / 2;

            if (max != min)
            {
                double d = max - min;
                s = l > 0.5 ? d / (2 - max - min) : d / (max + min);
                if (max == rNorm) h = (gNorm - bNorm) / d + (gNorm < bNorm ? 6 : 0);
                else if (max == gNorm) h = (bNorm - rNorm) / d + 2;
                else h = (rNorm - gNorm) / d + 4;
                h *= 60;
            }
            return ((int)h, (int)(s * 100), (int)(l * 100));
        }

        private (int r, int g, int b) HslToRgb(int h, int s, int l)
        {
            double sNorm = s / 100.0, lNorm = l / 100.0;
            double c = (1 - Math.Abs(2 * lNorm - 1)) * sNorm;
            double x = c * (1 - Math.Abs((h / 60.0) % 2 - 1));
            double m = lNorm - c / 2;
            double r = 0, g = 0, b = 0;

            if (h < 60) { r = c; g = x; }
            else if (h < 120) { r = x; g = c; }
            else if (h < 180) { g = c; b = x; }
            else if (h < 240) { g = x; b = c; }
            else if (h < 300) { r = x; b = c; }
            else { r = c; b = x; }

            return ((int)((r + m) * 255), (int)((g + m) * 255), (int)((b + m) * 255));
        }
    }
}
