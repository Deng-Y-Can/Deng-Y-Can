using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;

namespace WebApi.Services
{
    public class NumberService
    {
        public Dictionary<string, string> ConvertBase(string number, int fromBase, int toBase)
        {
            var value = BigInteger.Parse(number, new NumberFormatInfo { NumberGroupSeparator = "" });
            return new Dictionary<string, string>
            {
                ["decimal"] = value.ToString(),
                ["binary"] = Convert.ToString((long)value, 2),
                ["octal"] = Convert.ToString((long)value, 8),
                ["hexadecimal"] = Convert.ToString((long)value, 16).ToUpper()
            };
        }

        public string DecToBin(long dec) => Convert.ToString(dec, 2);
        public string DecToOct(long dec) => Convert.ToString(dec, 8);
        public string DecToHex(long dec) => Convert.ToString(dec, 16).ToUpper();
        public long BinToDec(string bin) => Convert.ToInt64(bin, 2);
        public long OctToDec(string oct) => Convert.ToInt64(oct, 8);
        public long HexToDec(string hex) => Convert.ToInt64(hex, 16);
        public string BinToOct(string bin) => Convert.ToString(Convert.ToInt64(bin, 2), 8);
        public string BinToHex(string bin) => Convert.ToString(Convert.ToInt64(bin, 2), 16).ToUpper();
        public string OctToBin(string oct) => Convert.ToString(Convert.ToInt64(oct, 8), 2);
        public string OctToHex(string oct) => Convert.ToString(Convert.ToInt64(oct, 8), 16).ToUpper();
        public string HexToBin(string hex) => Convert.ToString(Convert.ToInt64(hex, 16), 2);
        public string HexToOct(string hex) => Convert.ToString(Convert.ToInt64(hex, 16), 8);

        public Dictionary<string, object> RomanConvert(int number)
        {
            return new Dictionary<string, object>
            {
                ["number"] = number,
                ["roman"] = ToRoman(number),
                ["fromRoman"] = ToArabic(ToRoman(number))
            };
        }

        public string ToRoman(int number)
        {
            if (number < 1 || number > 3999) return "Out of range";
            var values = new[] { 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };
            var symbols = new[] { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };
            var result = new StringBuilder();
            for (int i = 0; i < values.Length; i++)
            {
                while (number >= values[i])
                {
                    result.Append(symbols[i]);
                    number -= values[i];
                }
            }
            return result.ToString();
        }

        public int ToArabic(string roman)
        {
            var map = new Dictionary<char, int>
            {
                ['I'] = 1, ['V'] = 5, ['X'] = 10, ['L'] = 50,
                ['C'] = 100, ['D'] = 500, ['M'] = 1000
            };
            int result = 0, prev = 0;
            foreach (var c in roman.ToUpper().Reverse())
            {
                if (!map.ContainsKey(c)) continue;
                int current = map[c];
                result += current < prev ? -current : current;
                prev = current;
            }
            return result;
        }

        public Dictionary<string, object> MathOperation(string op, double a, double b = 0)
        {
            return op.ToLower() switch
            {
                "add" or "+" => new Dictionary<string, object> { ["result"] = a + b, ["expression"] = $"{a} + {b} = {a + b}" },
                "sub" or "-" => new Dictionary<string, object> { ["result"] = a - b, ["expression"] = $"{a} - {b} = {a - b}" },
                "mul" or "*" => new Dictionary<string, object> { ["result"] = a * b, ["expression"] = $"{a} * {b} = {a * b}" },
                "div" or "/" => new Dictionary<string, object> { ["result"] = b != 0 ? a / b : (object)"undefined", ["expression"] = $"{a} / {b}" },
                "mod" or "%" => new Dictionary<string, object> { ["result"] = a % b, ["expression"] = $"{a} % {b} = {a % b}" },
                "pow" or "^" => new Dictionary<string, object> { ["result"] = Math.Pow(a, b), ["expression"] = $"{a} ^ {b} = {Math.Pow(a, b)}" },
                "sqrt" => new Dictionary<string, object> { ["result"] = Math.Sqrt(a), ["expression"] = $"sqrt({a}) = {Math.Sqrt(a)}" },
                "abs" => new Dictionary<string, object> { ["result"] = Math.Abs(a), ["expression"] = $"|{a}| = {Math.Abs(a)}" },
                "floor" => new Dictionary<string, object> { ["result"] = Math.Floor(a), ["expression"] = $"floor({a}) = {Math.Floor(a)}" },
                "ceil" => new Dictionary<string, object> { ["result"] = Math.Ceiling(a), ["expression"] = $"ceil({a}) = {Math.Ceiling(a)}" },
                "round" => new Dictionary<string, object> { ["result"] = Math.Round(a), ["expression"] = $"round({a}) = {Math.Round(a)}" },
                "sin" => new Dictionary<string, object> { ["result"] = Math.Sin(a), ["expression"] = $"sin({a}) = {Math.Sin(a)}" },
                "cos" => new Dictionary<string, object> { ["result"] = Math.Cos(a), ["expression"] = $"cos({a}) = {Math.Cos(a)}" },
                "tan" => new Dictionary<string, object> { ["result"] = Math.Tan(a), ["expression"] = $"tan({a}) = {Math.Tan(a)}" },
                "log" => new Dictionary<string, object> { ["result"] = Math.Log(a), ["expression"] = $"log({a}) = {Math.Log(a)}" },
                "log10" => new Dictionary<string, object> { ["result"] = Math.Log10(a), ["expression"] = $"log10({a}) = {Math.Log10(a)}" },
                "factorial" => new Dictionary<string, object> { ["result"] = Factorial((int)a), ["expression"] = $"{(int)a}! = {Factorial((int)a)}" },
                "fibonacci" => Fibonacci((int)a),
                _ => new Dictionary<string, object> { ["error"] = $"Unknown operation: {op}" }
            };
        }

        private long Factorial(int n)
        {
            if (n < 0) return -1;
            if (n <= 1) return 1;
            long result = 1;
            for (int i = 2; i <= n; i++) result *= i;
            return result;
        }

        private Dictionary<string, object> Fibonacci(int n)
        {
            var seq = new List<long>();
            long a = 0, b = 1;
            for (int i = 0; i < n; i++)
            {
                seq.Add(a);
                (a, b) = (b, a + b);
            }
            return new Dictionary<string, object>
            {
                ["n"] = n,
                ["sequence"] = seq,
                ["result"] = seq.LastOrDefault()
            };
        }

        public Dictionary<string, object> Statistics(List<double> numbers)
        {
            if (!numbers.Any())
                return new Dictionary<string, object> { ["error"] = "Empty list" };

            var sorted = numbers.OrderBy(n => n).ToList();
            return new Dictionary<string, object>
            {
                ["count"] = numbers.Count,
                ["sum"] = numbers.Sum(),
                ["mean"] = numbers.Average(),
                ["median"] = sorted.Count % 2 == 0
                    ? (sorted[sorted.Count / 2 - 1] + sorted[sorted.Count / 2]) / 2.0
                    : sorted[sorted.Count / 2],
                ["min"] = sorted.First(),
                ["max"] = sorted.Last(),
                ["range"] = sorted.Last() - sorted.First(),
                ["variance"] = numbers.Average(x => Math.Pow(x - numbers.Average(), 2)),
                ["stdDev"] = Math.Sqrt(numbers.Average(x => Math.Pow(x - numbers.Average(), 2)))
            };
        }

        public string NumberToWords(long number)
        {
            if (number == 0) return "zero";
            if (number < 0) return "minus " + NumberToWords(-number);

            var units = new[] { "", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
            var tens = new[] { "", "", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

            if (number < 20) return units[number];
            if (number < 100) return tens[number / 10] + (number % 10 != 0 ? "-" + units[number % 10] : "");
            if (number < 1000) return units[number / 100] + " hundred" + (number % 100 != 0 ? " and " + NumberToWords(number % 100) : "");
            if (number < 1000000) return NumberToWords(number / 1000) + " thousand" + (number % 1000 != 0 ? " " + NumberToWords(number % 1000) : "");
            if (number < 1000000000) return NumberToWords(number / 1000000) + " million" + (number % 1000000 != 0 ? " " + NumberToWords(number % 1000000) : "");
            return NumberToWords(number / 1000000000) + " billion" + (number % 1000000000 != 0 ? " " + NumberToWords(number % 1000000000) : "");
        }
    }
}
