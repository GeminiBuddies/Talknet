using System;
using System.Diagnostics;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Talknet;
using Talknet.Invoker;

namespace UnitTester {
    [TestClass]
    public class UnitTest0 {
        private static readonly Random _rand = new Random();

        [TestMethod]
        public void CommandInvokerBase() {
            var inv = new CommandInvoker();
            int flag = -1;

            inv.Register("aaa", () => { flag = 1; return 1; });
            inv.Register("bbb", (string str) => { flag = 2; return int.Parse(str); });
            inv.Register("ccc", (int i, int j) => { flag = 3; return i + j; });
            inv.Register("ddd", (int[] i) => { flag = 4; return i.Sum(); });

            Assert.AreEqual(inv.Invoke("aaa"), 1);
            Assert.AreEqual(flag, 1);
            Assert.AreEqual(inv.Invoke("bbb", "2333"), 2333);
            Assert.AreEqual(flag, 2);
            Assert.AreEqual(inv.Invoke("ccc", "2333", "2233"), 4566);
            Assert.AreEqual(flag, 3);
            Assert.AreEqual(inv.Invoke("ddd", "2333", "2233", "4396", "3154"), 2333 + 2233 + 4396 + 3154);
            Assert.AreEqual(flag, 4);
        }

        [TestMethod]
        public void CommandInvokerUpdate() {
            var inv = new CommandInvoker();
            int flag = -1;

            inv.Register("aaa", () => { flag = 1; return 1; });
            inv.Register("bbb", (string str) => { flag = 2; return int.Parse(str); });

            Assert.AreEqual(inv.Invoke("aaa"), 1);
            Assert.AreEqual(flag, 1);
            Assert.AreEqual(inv.Invoke("bbb", "2333"), 2333);
            Assert.AreEqual(flag, 2);

            inv.Update("aaa", () => { flag = 3; return 4; });
            inv.Update("bbb", (int[] arr) => { flag = 4; return arr.Sum(); });

            Assert.AreEqual(inv.Invoke("aaa"), 4);
            Assert.AreEqual(flag, 3);
            Assert.AreEqual(inv.Invoke("bbb", "2333", "2233"), 4566);
            Assert.AreEqual(flag, 4);
        }

        private class NnN { // "a:b"
            internal int A, B;

            public static NnN Parse(string v) {
                var vp = v.Split(':');

                if (vp.Length != 2 || !int.TryParse(vp[0], out var aa) || !int.TryParse(vp[1], out var bb)) throw new Exception();
                return new NnN { A = aa, B = bb };
            }
        }

        private const int Times = 32;
        [TestMethod]
        public void CommandInvokerCustomParser() {
            var inv = new CommandInvoker();
            inv.Register("abc", (NnN v) => v.A + v.B);

            for (var i = 0; i < Times; ++i) {
                int ra = _rand.Next(0, 1024), rb = _rand.Next(0, 1024);
                Trace.WriteLine($"Test with ra = {ra}, rb = {rb}");
                Assert.AreEqual(inv.Invoke("abc", $"{ra}:{rb}"), ra + rb);
            }

            inv.CustomParser(typeof(NnN), str => int.TryParse(str, out var v) ? new NnN { A = v, B = 100 } : throw new Exception());

            for (var i = 0; i < Times; ++i) {
                var r = _rand.Next(0, 1024);
                Trace.WriteLine($"Test with r = {r}");
                Assert.AreEqual(inv.Invoke("abc", $"{r}"), r + 100);
            }
        }
    }
}
