using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace WebApi.Services
{
    public class UtilityService
    {
        public string NewGuid(bool noDash = false)
        {
            var id = Guid.NewGuid().ToString();
            return noDash ? id.Replace("-", "") : id;
        }

        public string NewId(int length = 16)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[RandomNumberGenerator.GetInt32(s.Length)]).ToArray());
        }

        public long Timestamp()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        public long TimestampMs()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        public DateTime TimestampToDateTime(long timestamp, bool isMs = false)
        {
            if (isMs)
                return DateTimeOffset.FromUnixTimeMilliseconds(timestamp).LocalDateTime;
            return DateTimeOffset.FromUnixTimeSeconds(timestamp).LocalDateTime;
        }

        public long DateTimeToTimestamp(string dateTime)
        {
            if (DateTime.TryParse(dateTime, out var dt))
                return new DateTimeOffset(dt).ToUnixTimeSeconds();
            throw new FormatException("Invalid datetime format");
        }

        public Dictionary<string, object> GetTimeInfo(string timezone = "UTC")
        {
            var now = DateTime.Now;
            var utcNow = DateTime.UtcNow;
            return new Dictionary<string, object>
            {
                ["localTime"] = now.ToString("yyyy-MM-dd HH:mm:ss"),
                ["utcTime"] = utcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                ["timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                ["timestampMs"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                ["dayOfWeek"] = now.DayOfWeek.ToString(),
                ["dayOfYear"] = now.DayOfYear,
                ["weekOfYear"] = ISOWeek.GetWeekOfYear(now)
            };
        }

        public Dictionary<string, string> ParseJson(string json)
        {
            var dict = new Dictionary<string, string>();
            try
            {
                var obj = Newtonsoft.Json.Linq.JToken.Parse(json);
                if (obj is Newtonsoft.Json.Linq.JObject jObj)
                {
                    foreach (var prop in jObj.Properties())
                        dict[prop.Name] = prop.Value?.ToString();
                }
            }
            catch { }
            return dict;
        }

        public List<Dictionary<string, string>> ParseJsonArray(string json)
        {
            var list = new List<Dictionary<string, string>>();
            try
            {
                var arr = Newtonsoft.Json.Linq.JArray.Parse(json);
                foreach (var item in arr)
                {
                    if (item is Newtonsoft.Json.Linq.JObject jObj)
                    {
                        var dict = new Dictionary<string, string>();
                        foreach (var prop in jObj.Properties())
                            dict[prop.Name] = prop.Value?.ToString();
                        list.Add(dict);
                    }
                }
            }
            catch { }
            return list;
        }

        public string PrettyJson(string json)
        {
            try
            {
                var obj = Newtonsoft.Json.Linq.JToken.Parse(json);
                return obj.ToString(Newtonsoft.Json.Formatting.Indented);
            }
            catch
            {
                return json;
            }
        }

        public string MinifyJson(string json)
        {
            try
            {
                var obj = Newtonsoft.Json.Linq.JToken.Parse(json);
                return obj.ToString(Newtonsoft.Json.Formatting.None);
            }
            catch
            {
                return json;
            }
        }

        public string JsonToXml(string json)
        {
            try
            {
                var doc = Newtonsoft.Json.JsonConvert.DeserializeXmlNode(json, "root");
                using var sw = new StringWriter();
                doc.Save(sw);
                return sw.ToString();
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public string XmlToJson(string xml)
        {
            try
            {
                var doc = new System.Xml.XmlDocument();
                doc.LoadXml(xml);
                return Newtonsoft.Json.JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.Indented, true);
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public string RegexTest(string pattern, string input)
        {
            try
            {
                var matches = Regex.Matches(input, pattern);
                var result = matches.Cast<Match>().Select(m => new Dictionary<string, object>
                {
                    ["value"] = m.Value,
                    ["index"] = m.Index,
                    ["length"] = m.Length,
                    ["groups"] = m.Groups.Cast<Group>().Select(g => new { g.Value, g.Index, g.Length }).ToList()
                }).ToList();
                return Newtonsoft.Json.JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented);
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public string RegexReplace(string pattern, string input, string replacement)
        {
            return Regex.Replace(input, pattern, replacement);
        }

        public List<string> RegexSplit(string pattern, string input)
        {
            return Regex.Split(input, pattern).ToList();
        }
    }
}
