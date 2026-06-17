using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace WebApi.Services
{
    public class TextService
    {
        public Dictionary<string, object> Analyze(string text)
        {
            var chars = text.ToCharArray();
            var words = Regex.Matches(text, @"\b\w+\b");
            var lines = text.Split('\n');
            var paragraphs = text.Split(new[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.RemoveEmptyEntries);

            return new Dictionary<string, object>
            {
                ["characters"] = chars.Length,
                ["charactersNoSpace"] = chars.Count(c => !char.IsWhiteSpace(c)),
                ["words"] = words.Count,
                ["sentences"] = Regex.Matches(text, @"[.!?]+").Count,
                ["lines"] = lines.Length,
                ["paragraphs"] = paragraphs.Length,
                ["chineseChars"] = Regex.Matches(text, @"[\u4e00-\u9fff]").Count,
                ["numbers"] = Regex.Matches(text, @"\d+").Count,
                ["emails"] = Regex.Matches(text, @"[\w.-]+@[\w.-]+\.\w+").Count,
                ["urls"] = Regex.Matches(text, @"https?://\S+").Count,
                ["avgWordLength"] = words.Count > 0 ? Math.Round(words.Cast<Match>().Average(m => m.Value.Length), 2) : 0,
                ["uniqueWords"] = words.Cast<Match>().Select(m => m.Value.ToLower()).Distinct().Count()
            };
        }

        public string Reverse(string text) => new string(text.Reverse().ToArray());

        public string ToUpper(string text) => text.ToUpper();

        public string ToLower(string text) => text.ToLower();

        public string ToTitleCase(string text)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text.ToLower());
        }

        public string ToCamelCase(string text)
        {
            var words = Regex.Split(text, @"[\s_-]+");
            if (words.Length == 0) return "";
            return words[0].ToLower() + string.Join("", words.Skip(1).Select(w =>
                char.ToUpper(w[0]) + w.Substring(1).ToLower()));
        }

        public string ToSnakeCase(string text)
        {
            return Regex.Replace(text, @"([a-z])([A-Z])", "$1_$2").ToLower();
        }

        public string ToKebabCase(string text)
        {
            return Regex.Replace(text, @"([a-z])([A-Z])", "$1-$2").ToLower();
        }

        public string Truncate(string text, int maxLength, string suffix = "...")
        {
            if (text.Length <= maxLength) return text;
            return text.Substring(0, maxLength - suffix.Length) + suffix;
        }

        public string Repeat(string text, int count)
        {
            return string.Concat(Enumerable.Repeat(text, count));
        }

        public string PadLeft(string text, int totalWidth, char padChar = ' ')
        {
            return text.PadLeft(totalWidth, padChar);
        }

        public string PadRight(string text, int totalWidth, char padChar = ' ')
        {
            return text.PadRight(totalWidth, padChar);
        }

        public string Mask(string text, string pattern = "*")
        {
            if (string.IsNullOrEmpty(pattern)) pattern = "*";
            var sb = new StringBuilder();
            foreach (var c in text)
            {
                if (char.IsLetterOrDigit(c))
                    sb.Append(pattern[0]);
                else
                    sb.Append(c);
            }
            return sb.ToString();
        }

        public string MaskEmail(string email)
        {
            var parts = email.Split('@');
            if (parts.Length != 2) return email;
            var name = parts[0];
            var masked = name.Length <= 2
                ? new string('*', name.Length)
                : name[0] + new string('*', name.Length - 2) + name[name.Length - 1];
            return $"{masked}@{parts[1]}";
        }

        public string MaskPhone(string phone)
        {
            if (phone.Length <= 6) return phone;
            return phone.Substring(0, 3) + new string('*', phone.Length - 6) + phone.Substring(phone.Length - 3);
        }

        public List<string> ExtractEmails(string text)
        {
            return Regex.Matches(text, @"[\w.-]+@[\w.-]+\.\w+")
                .Cast<Match>().Select(m => m.Value).Distinct().ToList();
        }

        public List<string> ExtractUrls(string text)
        {
            return Regex.Matches(text, @"https?://\S+")
                .Cast<Match>().Select(m => m.Value).Distinct().ToList();
        }

        public List<string> ExtractPhones(string text)
        {
            return Regex.Matches(text, @"[\d+\-().\s]{7,20}")
                .Cast<Match>().Select(m => m.Value.Trim()).Distinct().ToList();
        }

        public string Diff(string text1, string text2)
        {
            var lines1 = text1.Split('\n');
            var lines2 = text2.Split('\n');
            var sb = new StringBuilder();
            int max = Math.Max(lines1.Length, lines2.Length);

            for (int i = 0; i < max; i++)
            {
                if (i >= lines1.Length)
                    sb.AppendLine($"+ {lines2[i]}");
                else if (i >= lines2.Length)
                    sb.AppendLine($"- {lines1[i]}");
                else if (lines1[i] != lines2[i])
                {
                    sb.AppendLine($"- {lines1[i]}");
                    sb.AppendLine($"+ {lines2[i]}");
                }
            }
            return sb.ToString();
        }

        public string RemoveDuplicates(string text)
        {
            var lines = text.Split('\n');
            return string.Join("\n", lines.Distinct());
        }

        public string SortLines(string text, bool descending = false)
        {
            var lines = text.Split('\n');
            return descending
                ? string.Join("\n", lines.OrderByDescending(l => l))
                : string.Join("\n", lines.OrderBy(l => l));
        }
    }
}
