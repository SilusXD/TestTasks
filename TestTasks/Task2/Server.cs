using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTasks.Task2
{
    internal static class Server
    {
        private static int count;

        public static int GetCount()
        {
            return count;
        }

        public static void AddToCount(int value)
        {
            count += value;
        }

    }
}
