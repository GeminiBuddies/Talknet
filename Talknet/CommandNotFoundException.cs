using System;

namespace Talknet {
    public class CommandNotFoundException : CommandInvokingException {
        public CommandNotFoundException(string command) : base(command) { }
    }
}
