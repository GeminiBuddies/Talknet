using System;
using System.Collections.Generic;
using System.Linq;

namespace Talknet {
    [Obsolete]
    public delegate T CommandHandler<T>(string cmd, string[] param);
    [Obsolete]
    internal class CommandInvoker<T> {
        readonly Dictionary<string, CommandHandler<T>> handlers;
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
