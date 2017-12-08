using System;
using System.Collections.Generic;
using System.Linq;

namespace Talknet {
    public struct CommandHandlerPair {
        public string Command;
        public Delegate Handler;
    }

    [Serializable]
    public class InvalidCommandHandlerException : Exception {
        // public InvalidCommandHandlerException() { }
        public InvalidCommandHandlerException(string message) : base(message) { }
        public InvalidCommandHandlerException(string message, Exception inner) : base(message, inner) { }
    }

    // do not implement override, now
    public class CommandInvoker {
        private struct PackedHandler {
            public Delegate Handler;
            public bool HasParamArray;
        }

        public delegate int DefaultHandler(params string[] value);

        private readonly Dictionary<string, PackedHandler> _handlers;
        private DefaultHandler _defaultHandler;

        public CommandInvoker() {
            _handlers = null;
        }

        public void Register(string command, Delegate handler) {
            if (_handlers.ContainsKey(command)) throw new ArgumentOutOfRangeException(nameof(command));

            RegisterOrUpdate(command, handler);
        }

        public void Register(params CommandHandlerPair[] pairs) {
            if (pairs == null) throw new ArgumentNullException(nameof(pairs));

            foreach (var i in pairs) Register(i.Command, i.Handler);
        }

        public void Update(string command, Delegate handler) {
            if (!_handlers.ContainsKey(command)) throw new ArgumentOutOfRangeException(nameof(command));

            RegisterOrUpdate(command, handler);
        }

        public void Update(params CommandHandlerPair[] pairs) {
            if (pairs == null) throw new ArgumentNullException(nameof(pairs));

            foreach (var i in pairs) Update(i.Command, i.Handler);
        }

        public void RegisterOrUpdate(string command, Delegate handler) => _handlers[command] = checkAndPackHandler(handler);

        public void Default(DefaultHandler handler) { _defaultHandler = handler; }

        private static PackedHandler checkAndPackHandler(Delegate handler) {
            // TODO: write a description
            if (handler.Method.ReturnType != typeof(int)) throw new InvalidCommandHandlerException("returntype");

            var parameters = handler.Method.GetParameters();
            // TODO: consider how to make this class works with optional parameters
            if (parameters.Any(param => param.IsOut || param.IsOptional) ||
                parameters.AllExceptLastOne().Any(param => param.ParameterType.IsArray))
                throw new InvalidCommandHandlerException("param");

            return new PackedHandler { Handler = handler, HasParamArray = parameters.Last().ParameterType.IsArray };
        }

        public int Invoke(string command, params object[] parameters) {
            throw new NotImplementedException();
        }

        public int InvokeFromLine(string line) {
            throw new NotImplementedException();
        }
    }
}
