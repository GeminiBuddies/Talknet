using System;
using System.Linq;
using System.Text;
using System.Linq;
using Talknet;

namespace Tester {
    internal class Program {
        private static int b(StringBuilder sb, int times) {
            for (var i = 0; i < times; ++i) sb.Append(" ha");
            Console.WriteLine(sb.ToString());
            return sb.Length;
        }

        public static void Main(string[] args) {
            var inv = new CommandInvoker();
            inv.Register("hhh", (Func<StringBuilder, int, int>)b);
            inv.Register("aaa", (Func<int[], int>)(i => i.Sum()));

            inv.InvokeFromLine("aaa 2333 2233 4396 3154");
        }
    }
}
