using System;
using System.Text;
using Talknet;

namespace Tester {
    internal class Program {
        private static int b(StringBuilder sb) {
            sb.Append("hahahaha");
            Console.WriteLine(sb.ToString());
            return sb.Length;
        }

        public static void Main(string[] args) {
            var inv = new CommandInvoker();
            inv.CustomParser(typeof(int), str => int.Parse(str));
            inv.Register("hhh", (Func<StringBuilder, int>)b);

            inv.InvokeFromLine("hhh asdiobasiodbfioasbfioabsfaosbdfioasdf");
        }
    }
}
