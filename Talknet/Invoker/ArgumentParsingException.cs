using System;
using Talknet.Invoker.i18n;

namespace Talknet.Invoker {
    public class ArgumentParsingException : CommandInvokingException {
        public Type TargetType { get; }
        public int ParamId { get; }
        public string Argument { get; }

        // Todo: write a desc
        public ArgumentParsingException(string command, Type targetType, int paramId, string argument, Exception innerException)
            : base(command, string.Format(CIErrMsg.ParsingExceptionDesc, paramId, argument, targetType), innerException) {
            TargetType = targetType;
            ParamId = paramId;
            Argument = argument;
        }
    }
}