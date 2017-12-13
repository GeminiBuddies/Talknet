using System;

namespace Talknet.Invoker {
    public class CommandExitAbnormallyException : CommandInvokingException {
        public CommandExitAbnormallyException(string command, Exception innerException) : base(command, null, innerException) { }
    }
}