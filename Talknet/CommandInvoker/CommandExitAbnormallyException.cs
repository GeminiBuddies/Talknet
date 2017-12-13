using System;

namespace Talknet.CommandInvoker {
    public class CommandExitAbnormallyException : CommandInvokingException {
        public CommandExitAbnormallyException(string command, Exception innerException) : base(command, null, innerException) { }
    }
}