using System;
using Talknet.CommandInvoker.i18n;

namespace Talknet.CommandInvoker {
    public class DoNotKnowHowToParseException : CommandInvokingException {
        public Type TargetType { get; }
        
        public DoNotKnowHowToParseException(Type targetType, string command) 
            : base(string.Format(CIErrMsg.IDKParseExceptionDesc, targetType.FullName, command))
            => TargetType = targetType;
    }
}
