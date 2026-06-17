using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace WebApi.Services
{
    public class EncodeService
    {
        public string Base64Encode(string input) => Convert.ToBase64String(Encoding.UTF8.GetBytes(input));

        public string Base64Decode(string input) => Encoding.UTF8.GetString(Convert.FromBase64String(input));

        public string UrlEncode(string input) => Uri.EscapeDataString(input);

        public string UrlDecode(string input) => Uri.UnescapeDataString(input);

        public string HtmlEncode(string input) => System.Net.WebUtility.HtmlEncode(input);

        public string HtmlDecode(string input) => System.Net.WebUtility.HtmlDecode(input);

        public string UnicodeEncode(string input)
        {
            var sb = new StringBuilder();
            foreach (char c in input)
                sb.Append($"\\u{(int)c:x4}");
            return sb.ToString();
        }

        public string UnicodeDecode(string input)
        {
            return Regex.Replace(input, @"\\u([0-9a-fA-F]{4})", match =>
            {
                return ((char)int.Parse(match.Groups[1].Value, NumberStyles.HexNumber)).ToString();
            });
        }

        public string HexEncode(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }

        public string HexDecode(string input)
        {
            var bytes = new byte[input.Length / 2];
            for (int i = 0; i < input.Length; i += 2)
                bytes[i / 2] = Convert.ToByte(input.Substring(i, 2), 16);
            return Encoding.UTF8.GetString(bytes);
        }

        public string Utf8ToGbk(string input)
        {
            var utf8 = Encoding.UTF8.GetBytes(input);
            var gbk = Encoding.GetEncoding("GBK");
            return gbk.GetString(utf8);
        }

        public string GbkToUtf8(string input)
        {
            var gbk = Encoding.GetEncoding("GBK");
            var bytes = gbk.GetBytes(input);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
