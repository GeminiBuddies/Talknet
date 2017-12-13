// multi-connection is still not implemented 

using System;
using Talknet.Invoker;

namespace Talknet {
    public class TalknetEnv {
        public Action<string> RemoteSetter;
        public TalknetTcpClient Client;
        public CommandInvoker Invoker;
        public Action Exiter;
    }
}

