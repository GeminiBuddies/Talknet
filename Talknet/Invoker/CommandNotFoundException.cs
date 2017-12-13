namespace Talknet.Invoker {
    public class CommandNotFoundException : CommandInvokingException {
        public CommandNotFoundException(string command) : base(command) { }
    }
}
