using Talknet.CommandInvoker.i18n;

namespace Talknet.CommandInvoker {
    public class CommandArgumentCountException : CommandInvokingException {
        public CommandArgumentCountException(string command) : base(command, string.Format(CIErrMsg.ArgCountExceptionDesc, command)) { }
        public CommandArgumentCountException(string command, string message) : base(command, message) { }
    }
}
