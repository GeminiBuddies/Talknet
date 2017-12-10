using System;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Talknet;

namespace UnitTester {
    [TestClass]
    public class UnitTest0 {
        [TestMethod]
        public void CommandInvokerBase() {
            var inv = new CommandInvoker();
            int flag = -1;

            inv.Register("aaa", (Func<int>)(() => { flag = 1; return 1; }));
            inv.Register("bbb", (Func<string, int>)((str) => { flag = 2; return int.Parse(str); }));
            inv.Register("ccc", (Func<int, int>)((i) => { flag = 3; return i; }));
            inv.Register("ddd", (Func<int[], int>)((i) => { flag = 4; return i.Sum(); }));

            Assert.AreEqual(inv.Invoke("aaa"), 1);
            Assert.AreEqual(flag, 1);
            Assert.AreEqual(inv.Invoke("bbb", "2333"), 2333);
            Assert.AreEqual(flag, 2);
            Assert.AreEqual(inv.Invoke("ccc", "2333"), 2333);
            Assert.AreEqual(flag, 3);
            Assert.AreEqual(inv.Invoke("ddd", "2333", "2233", "4396", "3154"), 2333 + 2233 + 4396 + 3154);
            Assert.AreEqual(flag, 4);
        }

        [TestMethod]
        public void CommandInvokerUpdate() {
            var inv = new CommandInvoker();
            int flag = -1;

            inv.Register("aaa", (Func<int>)(() => { flag = 1; return 1; }));
            inv.Register("bbb", (Func<string, int>)((str) => { flag = 2; return int.Parse(str); }));

            Assert.AreEqual(inv.Invoke("aaa"), 1);
            Assert.AreEqual(flag, 1);
            Assert.AreEqual(inv.Invoke("bbb", "2333"), 2333);
            Assert.AreEqual(flag, 2);

            inv.Update("aaa", (Func<int>)(() => { flag = 3; return 4; }));
            inv.Update("bbb", (Func<int[], int>)((arr) => { flag = 4; return arr.Sum(); }));

            Assert.AreEqual(inv.Invoke("aaa"), 4);
            Assert.AreEqual(flag, 3);
            Assert.AreEqual(inv.Invoke("bbb", "2333", "2233"), 4566);
            Assert.AreEqual(flag, 4);
        }
    }
}
