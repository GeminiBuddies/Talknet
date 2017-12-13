// multi-connection is still not implemented 

using System;
using System.Collections.Generic;
using System.Text;
using Talknet.Invoker;

namespace Talknet {
    public class TalknetEnv {
        public Action<string> RemoteSetter;
        public TalknetTcpClient Client;
        public CommandInvoker Invoker;
    }
}

