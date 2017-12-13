using System;
using Talknet.Invoker.i18n;

namespace Talknet.Invoker {
    public class DoNotKnowHowToParseException : CommandInvokingException {
        public Type TargetType { get; }
        
        public DoNotKnowHowToParseException(Type targetType, string command) 
            : base(string.Format(CIErrMsg.IDKParseExceptionDesc, targetType.FullName, command))
            => TargetType = targetType;
    }
}
