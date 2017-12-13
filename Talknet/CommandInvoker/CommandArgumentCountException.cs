namespace Talknet.CommandInvoker {
    public class CommandArgumentCountException : CommandInvokingException {
        // Todo: write a desc
        public CommandArgumentCountException(string command) : base(command) { }
        public CommandArgumentCountException(string command, string message) : base(command, message) { }
    }
}
