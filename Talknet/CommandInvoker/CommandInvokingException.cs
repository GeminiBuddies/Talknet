using System;

namespace Talknet.CommandInvoker {
    public class CommandInvokingException : Exception {
        public string Command { get; }

        public CommandInvokingException(string command) { Command = command; }
        public CommandInvokingException(string command, string message) : base(message) { Command = command; }
        public CommandInvokingException(string command, string message, Exception inner) : base(message, inner) { Command = command; }
    }
}
