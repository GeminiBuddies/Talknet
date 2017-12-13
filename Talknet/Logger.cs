using System;
using System.Diagnostics;
using System.Reflection;
using System.Linq;

namespace Talknet {
    public static class Logger {
        public static void Write(string value) => Exconsole.Write(value);
        public static void WriteLine(string value) => Exconsole.WriteLine(value);

        public static void WriteHighlight(string value) => Exconsole.Write("@c" + value + "@!");
        public static void WriteLineHighlight(string value) => Exconsole.Write("@c" + value + "@!\n");

        private static string getCallerInfo() {
            var sf = new StackFrame(2, true); // this|requester|caller
            var caller = sf.GetMethod();

            var callerInfo = $"[{caller.DeclaringType.Assembly.GetName().Name}]{caller.DeclaringType.FullName}::{caller.Name}({caller.GetParameters().Select(p => p.ParameterType.Name).JoinBy(", ")})";

            var fileName = sf.GetFileName();
            if (!string.IsNullOrEmpty(fileName)) callerInfo += $" {fileName}: {sf.GetFileLineNumber()}";
            return callerInfo;
        }

        public static void Error(string message, string callerInfo = null) {
            var sf = new StackFrame(1, true);
            var caller = sf.GetMethod();
            if (callerInfo == null) callerInfo = getCallerInfo();

            var log = $"[{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff}][Error]({callerInfo})\n{message}";

            Exconsole.WriteLine("@r" + message + "@!");
            // Exconsole.WriteLine("@m" + log + "@!");
        }

        public static void ErrorMultiline(params string[] messages) {
            Error(messages.JoinBy(Environment.NewLine), getCallerInfo());
        }

        public static void ErrorMultilineWithCaller(string callerInfo, params string[] messages) {
            Error(messages.JoinBy(Environment.NewLine), callerInfo);
        }
    }
}