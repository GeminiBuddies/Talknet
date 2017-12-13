using Talknet.Invoker.i18n;

namespace Talknet.Invoker {
    public class CommandArgumentCountException : CommandInvokingException {
        public CommandArgumentCountException(string command) : base(command, string.Format(CIErrMsg.ArgCountExceptionDesc, command)) { }
        public CommandArgumentCountException(string command, string message) : base(command, message) { }
    }
}
