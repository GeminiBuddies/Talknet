using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Talknet {
    public struct CommandHandlerPair {
        public string Command;
        public Delegate Handler;
        public string Description;
    }

    [Serializable]
    public class InvalidCommandHandlerException : Exception {
        // public InvalidCommandHandlerException() { }
        public InvalidCommandHandlerException(string message) : base(message) { }
        public InvalidCommandHandlerException(string message, Exception inner) : base(message, inner) { }
    }

    // do not implement override, now
    public class CommandInvoker {
        protected const string CastOperatorNameImplicit = "op_Implicit";
        protected const string CastOperatorNameExplicit = "op_Explicit";
        protected const string ParseFunctionName = "Parse";

        public delegate object Parser(string src);
        // custom parsers
        private readonly Dictionary<Type, Parser> _parserDict;
        // for ctor and implicit, explicit conversion operator
        private static readonly Dictionary<Type, Parser> _defaultParserDict = new Dictionary<Type, Parser>() {
            [typeof(string)] = str => str,
            [typeof(char)] = str => str.Length == 1 ? str[0] : throw new FormatException(),
        };

        private struct PackedHandler {
            public Delegate Handler;
            public ParameterInfo[] Parameters;
            public bool HasParamArray;
            public Type ParamArrayType;
        }

        public delegate int DefaultHandler(params string[] value);

        private readonly Dictionary<string, PackedHandler> _handlers;
        private readonly Dictionary<string, string> _desc;
        private DefaultHandler _defaultHandler;

        public CommandInvoker() {
            _parserDict = new Dictionary<Type, Parser>();
            _handlers = new Dictionary<string, PackedHandler>();
            _desc = new Dictionary<string, string>();
            _defaultHandler = null;
        }

        public void Register(string command, Delegate handler) {
            if (_handlers.ContainsKey(command)) throw new ArgumentOutOfRangeException(nameof(command));
            RegisterOrUpdate(command, handler);
        }

        public void Register(string command, Delegate handler, string desc) {
            if (_handlers.ContainsKey(command)) throw new ArgumentOutOfRangeException(nameof(command));
            RegisterOrUpdate(command, handler, desc);
        }

        public void Register(params CommandHandlerPair[] pairs) {
            if (pairs == null) throw new ArgumentNullException(nameof(pairs));
            foreach (var i in pairs) Register(i.Command, i.Handler, i.Description);
        }

        public void Update(string command, Delegate handler) {
            if (!_handlers.ContainsKey(command)) throw new ArgumentOutOfRangeException(nameof(command));
            RegisterOrUpdate(command, handler);
        }

        public void Update(string command, Delegate handler, string desc) {
            if (!_handlers.ContainsKey(command)) throw new ArgumentOutOfRangeException(nameof(command));
            RegisterOrUpdate(command, handler, desc);
        }

        public void Update(params CommandHandlerPair[] pairs) {
            if (pairs == null) throw new ArgumentNullException(nameof(pairs));
            foreach (var i in pairs) Update(i.Command, i.Handler, i.Description);
        }

        public void RegisterOrUpdate(string command, Delegate handler) => RegisterOrUpdate(command, handler, genDefaultDesc(command, handler));
        public void RegisterOrUpdate(string command, Delegate handler, string desc) {
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            _handlers[command] = checkAndPackHandler(handler);
            _desc[command] = desc;
        }

        public void Default(DefaultHandler handler) { _defaultHandler = handler; }

        public void CustomParser(Type destType, Parser parser) {
            if (destType == null) throw new ArgumentNullException(nameof(destType));
            _parserDict[destType] = parser ?? throw new ArgumentNullException(nameof(parser));
        }

        public string GetDescription(string command) => 
            _desc.ContainsKey(command)
            ? _desc[command]
            : throw new ArgumentOutOfRangeException(nameof(command));

        private static string genDefaultDesc(string command, Delegate handler) {
            return command + " " + handler.Method.GetParameters().Select(info => info.ParameterType.Name).JoinBy(" ");
        }

        private static void tryGenDefaultParser(Type destType) {
            if (_defaultParserDict.ContainsKey(destType)) return;

            Type[] paramList = { typeof(string) };
            if (destType.GetConstructor(paramList) is ConstructorInfo ctor) {
                // is there a constructor from string?
                _defaultParserDict[destType] = str => ctor.Invoke(new object[] { str });
            } else if (destType.GetMethod(CastOperatorNameExplicit, paramList) is MethodInfo opExp) {
                // or an explicit operator
                _defaultParserDict[destType] = str => opExp.Invoke(null, new object[] { str });
            } else if (destType.GetMethod(CastOperatorNameImplicit, paramList) is MethodInfo opImp) {
                // or an implicit operator
                _defaultParserDict[destType] = str => opImp.Invoke(null, new object[] { str });
            } else if (destType.GetMethod(ParseFunctionName, BindingFlags.Static | BindingFlags.Public, null, paramList, null) is MethodInfo p) {
                // or a 'static T T.Parse(string)'
                _defaultParserDict[destType] = str => p.Invoke(null, new object[] { str });
            } else {
                // or just give up
                _defaultParserDict[destType] = null;
            }
        }

        private static PackedHandler checkAndPackHandler(Delegate handler) {
            // TODO: write a description
            if (handler.Method.ReturnType != typeof(int)) throw new InvalidCommandHandlerException("returntype");

            var parameters = handler.Method.GetParameters();
            // TODO: consider how to make this class works with optional parameters
            if (parameters.Any(param => param.IsOut || param.IsOptional) ||
                parameters.AllExceptLastOne().Any(param => param.ParameterType.IsArray))
                throw new InvalidCommandHandlerException("param");

            bool hasParamArray = parameters.Length > 0 && parameters.Last().ParameterType.IsArray;

            var rv = new PackedHandler {
                Handler = handler,
                Parameters = hasParamArray ? parameters.AllExceptLastOne().ToArray() : parameters,
                HasParamArray = hasParamArray,
                ParamArrayType = hasParamArray ? parameters.Last().ParameterType.GetElementType() : null
            };

            foreach (var paramType in rv.Parameters) tryGenDefaultParser(paramType.ParameterType);
            if (rv.HasParamArray) tryGenDefaultParser(rv.ParamArrayType);

            return rv;
        }

        public int Invoke(string command, params string[] arguments) {
            if (!_handlers.ContainsKey(command)) {
                return _defaultHandler?.Invoke() ?? throw new Exception();
            }

            var handler = _handlers[command];
            if (!handler.HasParamArray && arguments.Length != handler.Parameters.Length)
                throw new TargetParameterCountException();
            if (handler.HasParamArray && arguments.Length < handler.Parameters.Length)
                throw new TargetParameterCountException();

            // prepare args
            var args = new List<object>();
            var count = handler.Parameters.Length;
            for (var i = 0; i < count; ++i) {
                var destType = handler.Parameters[i].ParameterType;
                if (_parserDict.ContainsKey(destType)) { // is there a parser
                    args.Add(_parserDict[destType](arguments[i]));
                } else if (_defaultParserDict.TryGetValue(destType, out var parser) && parser != null) { // or a default parser
                    args.Add(parser(arguments[i]));
                } else throw new Exception(); // Todo
            }

            if (handler.HasParamArray) {
                var paramArray = new List<object>();
                var argcount = arguments.Length;
                var paramArrayType = handler.ParamArrayType;

                if (!_parserDict.TryGetValue(paramArrayType, out var parser) &&
                    !(_defaultParserDict.TryGetValue(paramArrayType, out parser) && parser != null)) throw new Exception(); // Todo

                for (var i = count; i < argcount; ++i) paramArray.Add(parser(arguments[i]));

                args.Add(paramArray);
            }

            return (int)handler.Handler.DynamicInvoke(args.ToArray());
        }

        public int InvokeFromLine(string line) {
            if (line == null) throw new ArgumentNullException(nameof(line));
            List<string> tokens = LineTokenizer.GetTokens(line);

            if (tokens.Count < 1) return 0;
            return Invoke(tokens[0], tokens.Take(1, tokens.Count - 1).ToArray());
        }
    }
}
