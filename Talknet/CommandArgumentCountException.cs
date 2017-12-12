namespace Talknet {
    public class CommandArgumentCountException : CommandInvokingException {
        public CommandArgumentCountException(string command) : base(command) { }
        public CommandArgumentCountException(string command, string message) : base(command, message) { }
    }
}
