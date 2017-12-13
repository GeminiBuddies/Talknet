namespace Talknet.CommandInvoker {
    public class CommandNotFoundException : CommandInvokingException {
        public CommandNotFoundException(string command) : base(command) { }
    }
}
