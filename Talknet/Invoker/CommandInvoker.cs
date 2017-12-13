using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Talknet.Invoker {
    // do not implement override, now
    public partial class CommandInvoker {
        protected const string CastOperatorNameImplicit = "op_Implicit";
        protected const string CastOperatorNameExplicit = "op_Explicit";
        protected const string ParseFunctionName = "Parse";
        protected const string AddFunctionName = "Add";
        protected const string ToArrayFunctionName = "ToArray";

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

        private readonly Dictionary<string, PackedHandler> _handlers;
        private readonly Dictionary<string, string> _alterTable;

        public delegate int DefaultHandler(params string[] value);
        private DefaultHandler _defaultHandler;

        public CommandInvoker() {
            _parserDict = new Dictionary<Type, Parser>();
            _handlers = new Dictionary<string, PackedHandler>();
            _alterTable = new Dictionary<string, string>();
            _defaultHandler = null;
        }

        public void Register(string command, Delegate handler, params string[] alternativeForms) {
            if (_handlers.ContainsKey(command)) throw new ArgumentOutOfRangeException(nameof(command));
            RegisterOrUpdate(command, handler);

            foreach (var alter in alternativeForms) _alterTable[alter] = command;
        }

        public void Update(string command, Delegate handler) {
            if (!_handlers.ContainsKey(command)) throw new ArgumentOutOfRangeException(nameof(command));
            RegisterOrUpdate(command, handler);
        }

        public void RegisterOrUpdate(string command, Delegate handler) {
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            _handlers[command] = checkAndPackHandler(handler);
        }

        public void Default(DefaultHandler handler) { _defaultHandler = handler; }

        public void CustomParser(Type destType, Parser parser) {
            if (destType == null) throw new ArgumentNullException(nameof(destType));
            _parserDict[destType] = parser ?? throw new ArgumentNullException(nameof(parser));
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
            if (handler.Method.ReturnType != typeof(int)) throw new InvalidCommandHandlerException("Return type of handler must be int.");

            var parameters = handler.Method.GetParameters();
            // nolonger consider how to make this class works with optional parameters
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

        private static object doParse(Parser parser, string str, string command, int paramId, Type targetType) {
            try {
                return parser(str);
            } catch (Exception ex) {
                throw new ArgumentParsingException(command, targetType, paramId, str, ex);
            }
        }

        private Parser getParser(Type targetType, string command) {
            if (!_parserDict.TryGetValue(targetType, out var parser) &&
                !(_defaultParserDict.TryGetValue(targetType, out parser) && parser != null))
                throw new DoNotKnowHowToParseException(targetType, command);

            return parser;
        }

        public int Invoke(string command, params string[] arguments) {
            string inputCommand = command;
            if (!_handlers.ContainsKey(command)) {
                if (_alterTable.ContainsKey(command))
                    command = _alterTable[command];
                else
                    return _defaultHandler?.Invoke() ?? throw new CommandNotFoundException(command);
            }

            var handler = _handlers[command];
            if (!handler.HasParamArray && arguments.Length != handler.Parameters.Length)
                throw new CommandArgumentCountException(inputCommand);
            if (handler.HasParamArray && arguments.Length < handler.Parameters.Length)
                throw new CommandArgumentCountException(inputCommand);

            // prepare args
            var args = new List<object>();
            var count = handler.Parameters.Length;
            for (var i = 0; i < count; ++i) {
                var destType = handler.Parameters[i].ParameterType;

                args.Add(doParse(getParser(destType, command), arguments[i], command, i, destType));
            }

            if (handler.HasParamArray) {
                var argcount = arguments.Length;
                var paramArrayType = handler.ParamArrayType;
                var paramArrayListType = typeof(List<>).MakeGenericType(paramArrayType);
                var paramArray = Activator.CreateInstance(paramArrayListType);
                var paramArrayListAdder = paramArrayListType.GetMethod(AddFunctionName, new[] { paramArrayType });
                var paramArrayListToArray = paramArrayListType.GetMethod(ToArrayFunctionName, new Type[] { });

                var parser = getParser(paramArrayType, command);
                for (var i = count; i < argcount; ++i)
                    paramArrayListAdder.Invoke(paramArray, new[] { doParse(parser, arguments[i], command, i, paramArrayType) });

                args.Add(paramArrayListToArray.Invoke(paramArray, new object[] { }));
            }

            try {
                return (int) handler.Handler.DynamicInvoke(args.ToArray());
            } catch (TargetInvocationException ex) {
                if (ex.InnerException == null) throw;
                var ine = ex.InnerException;

                if (ine is TalknetCommandException) throw ine;
                throw new CommandExitAbnormallyException(inputCommand, ine);
            }
        }

        public int InvokeFromLine(string line) {
            if (line == null) throw new ArgumentNullException(nameof(line));
            var tokens = LineTokenizer.GetTokens(line);

            return tokens.Count < 1 ? 0 : Invoke(tokens[0], tokens.Take(1, tokens.Count - 1).ToArray());
        }
    }
}
