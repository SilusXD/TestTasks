using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTasks.Task1
{
    internal class StringCompressor
    {
        public static string Compress(string baseStr)
        {
            if (string.IsNullOrEmpty(baseStr))
            {
                throw new ArgumentException($"Argument {nameof(baseStr)} is Null or empty");
            }

            int count = 1;
            char ch = baseStr[0];
            StringBuilder compressedStr = new StringBuilder();

            for (int i = 1; i < baseStr.Length; i++)
            {
                if (baseStr[i] == ch)
                {
                    count += 1;
                }
                else
                {
                    compressedStr.Append(ch);
                    if (count > 1)
                    {
                        compressedStr.Append(count);
                    }

                    ch = baseStr[i];
                    count = 1;
                }
            }

            compressedStr.Append(ch);
            if (count > 1)
            {
                compressedStr.Append(count);
            }

            return compressedStr.ToString();
        }
        public static string Decompress(string compressedStr)
        {
            if (string.IsNullOrEmpty(compressedStr))
            {
                throw new ArgumentException($"Argument \"compressedStr\" is Null or empty");
            }

            StringBuilder decompressedStr = new StringBuilder();

            int i = 0;
            while (i < compressedStr.Length)
            {
                char ch = compressedStr[i];
                int count = 1;
                i++;

                if (i < compressedStr.Length && char.IsDigit(compressedStr[i]))
                {
                    count = 0;
                    while (i < compressedStr.Length && char.IsDigit(compressedStr[i]))
                    {
                        count = count * 10 + (compressedStr[i] - '0');
                        i++;
                    }
                }

                decompressedStr.Append(ch, count);
            }

            return decompressedStr.ToString();
        }
    }
}
