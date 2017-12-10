using System;
using System.Text;
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

            inv.InvokeFromLine("hhh asdiobasiodbfioasbfioabsfaosbdfioasdf 233");
        }
    }
}
