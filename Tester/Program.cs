using System;
using System.Linq;
using System.Text;
using Talknet;
using Talknet.Invoker;

namespace Tester {
    internal class Program {
        private static int b(StringBuilder sb, int times) {
            for (var i = 0; i < times; ++i) sb.Append(" ha");
            Console.WriteLine(sb.ToString());
            return sb.Length;
        }

        public static void Main(string[] args) {
            var inv = new CommandInvoker();
            inv.Register<StringBuilder, int>("hhh", b);
            inv.Register("aaa", (int[] i) => i.Sum());

            inv.InvokeFromLine("aaa 2333 2233 4396 3154");
        }
    }
}
