using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTasks
{
    internal class Task1_StringCompressor
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

    }
}
