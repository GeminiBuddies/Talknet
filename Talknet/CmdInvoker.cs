using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talknet.i18n;

namespace Talknet {
    internal static class Ext {
        internal static IEnumerable<T> Take<T>(this IEnumerable<T> src, int from, int count) {
            var en = src.GetEnumerator();

            for (int i = 0; i < from; ++i) {
                if (!en.MoveNext()) yield break;
            }

            for (int i = 0; i < count; ++i) {
                if (!en.MoveNext()) yield break;
                yield return en.Current;
            }
        }
    }

    internal class CommandNotFoundException : Exception {
        public string Command { get; private set; }

        public CommandNotFoundException(string command)
            : base($"Command \"{command}\" not expected and no default handler provided.") {
            Command = command;
        }
    }

    internal class CommandArgumentException : Exception {
        public string Command { get; private set; }
        public string Description { get; private set; }

        public CommandArgumentException(string command, string description)
            : base($"Incorrect argument for command \"{command}\": {description}") {
            Command = command;
            Description = description;
        }
    }

    public delegate T CommandHandler<T>(string cmd, string[] param);
    internal class CommandInvoker<T> {
        Dictionary<string, CommandHandler<T>> handlers;
        CommandHandler<T> defaultHandler = null;

        public CommandInvoker() {
            handlers = new Dictionary<string, CommandHandler<T>>();
        }

        public CommandInvoker<T> Register(string cmd, CommandHandler<T> handler) {
            lock (handlers) handlers[cmd] = handler;
            return this;
        }

        public CommandInvoker<T> RegisterNew(string cmd, CommandHandler<T> handler) {
            lock (handlers) handlers.Add(cmd, handler);
            return this;
        }

        public CommandInvoker<T> Unregister(string cmd, CommandHandler<T> handler) {
            lock (handlers) handlers.Remove(cmd);
            return this;
        }

        public CommandInvoker<T> Default(CommandHandler<T> handler) {
            defaultHandler = handler;
            return this;
        }

        public T Invoke(string cmd, string[] param = null) {
            param = param ?? new string[] { };

            var hdl = defaultHandler;
            lock (handlers) {
                if (!handlers.ContainsKey(cmd)) {
                    if (hdl == null) throw new CommandNotFoundException(cmd);
                } else {
                    hdl = handlers[cmd];
                }
            }

            return hdl(cmd, param);
        }

        public T InvokeFromLine(string line) {
            if (line == null) throw new ArgumentNullException(nameof(line));
            List<string> cache = LineTokenizer.GetTokens(line);

            if (cache.Count < 1) throw new Exception($"Cannot parse line \"{line}\"");

            return Invoke(cache[0], cache.Take(1, cache.Count - 1).ToArray());
        }
    }
}
