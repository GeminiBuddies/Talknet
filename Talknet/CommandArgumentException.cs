using System;

namespace Talknet {
    internal class CommandArgumentException : CommandInvokingException {
        public string Command { get; private set; }
        public string Description { get; private set; }

        public CommandArgumentException(string command, string description)
            : base($"Incorrect argument for command \"{command}\": {description}") {
            Command = command;
            Description = description;
        }
    }
}