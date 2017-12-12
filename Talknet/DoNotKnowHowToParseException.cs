using System;

namespace Talknet {
    public class DoNotKnowHowToParseException : CommandInvokingException {
        public Type TargetType { get; }

        public DoNotKnowHowToParseException(Type destType) : base("") => TargetType = destType;
        public DoNotKnowHowToParseException(Type destType, string command) : base(command) => TargetType = destType;
    }
}
