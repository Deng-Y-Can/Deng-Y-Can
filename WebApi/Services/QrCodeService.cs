using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace WebApi.Services
{
    public class QrCodeService
    {
        public string GenerateSvg(string text, int moduleSize = 10, string foreground = "#000000", string background = "#FFFFFF")
        {
            var matrix = EncodeToMatrix(text);
            var width = matrix.GetLength(0) * moduleSize;
            var height = matrix.GetLength(1) * moduleSize;

            var sb = new StringBuilder();
            sb.AppendLine($"<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"{width}\" height=\"{height}\" viewBox=\"0 0 {width} {height}\">");
            sb.AppendLine($"<rect width=\"{width}\" height=\"{height}\" fill=\"{background}\"/>");

            for (int y = 0; y < matrix.GetLength(1); y++)
            {
                for (int x = 0; x < matrix.GetLength(0); x++)
                {
                    if (matrix[x, y] == 1)
                    {
                        sb.AppendLine($"<rect x=\"{x * moduleSize}\" y=\"{y * moduleSize}\" width=\"{moduleSize}\" height=\"{moduleSize}\" fill=\"{foreground}\"/>");
                    }
                }
            }

            sb.AppendLine("</svg>");
            return sb.ToString();
        }

        public byte[] GeneratePng(string text, int moduleSize = 10)
        {
            var svg = GenerateSvg(text, moduleSize);
            return Encoding.UTF8.GetBytes(svg);
        }

        private int[,] EncodeToMatrix(string text)
        {
            var bits = GetBitsFromText(text);
            var size = CalculateSize(bits.Length);
            var matrix = new int[size, size];

            int pos = 0;
            for (int i = 0; i < bits.Length && pos < size * size; i++)
            {
                int x = pos % size;
                int y = pos / size;
                matrix[x, y] = bits[i] ? 1 : 0;
                pos++;
            }

            AddFinderPatterns(matrix, size);

            return matrix;
        }

        private bool[] GetBitsFromText(string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            var bits = new List<bool>();
            foreach (var b in bytes)
            {
                for (int i = 7; i >= 0; i--)
                    bits.Add((b & (1 << i)) != 0);
            }
            return bits.ToArray();
        }

        private int CalculateSize(int dataBits)
        {
            int minModules = (int)Math.Ceiling(Math.Sqrt(dataBits)) + 2;
            return Math.Max(21, (minModules / 4 + 1) * 4 + 1);
        }

        private void AddFinderPatterns(int[,] matrix, int size)
        {
            DrawFinderPattern(matrix, 0, 0);
            DrawFinderPattern(matrix, size - 7, 0);
            DrawFinderPattern(matrix, 0, size - 7);

            for (int i = 7; i < size - 7; i++)
            {
                matrix[i, 6] = i % 2 == 0 ? 1 : 0;
                matrix[6, i] = i % 2 == 0 ? 1 : 0;
            }
        }

        private void DrawFinderPattern(int[,] matrix, int x, int y)
        {
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    bool isBorder = i == 0 || i == 6 || j == 0 || j == 6;
                    bool isInner = i >= 2 && i <= 4 && j >= 2 && j <= 4;
                    matrix[x + i, y + j] = (isBorder || isInner) ? 1 : 0;
                }
            }
        }
    }
}
