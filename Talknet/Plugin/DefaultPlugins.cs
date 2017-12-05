using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Talknet.Plugin;

[assembly: TalknetPluginAssembly(typeof(DefaultPlugin))]

namespace Talknet.Plugin {
    [TalknetPlugin("geminilab.default", "Default", "1.0.0.0", "Gemini Laboratory")]
    class DefaultPlugin : ITalknetPlugin {
        public DefaultPlugin() { }

        public new void Finalize() {
        }

        public void Initialize(TalknetEnv env) {
        }
    }
}
