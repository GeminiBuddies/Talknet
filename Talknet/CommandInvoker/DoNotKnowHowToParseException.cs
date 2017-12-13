using System;

namespace Talknet.CommandInvoker {
    public class DoNotKnowHowToParseException : CommandInvokingException {
        public Type TargetType { get; }

        // Todo: write a desc
        public DoNotKnowHowToParseException(Type destType, string command) : base(command) => TargetType = destType;
    }
}
