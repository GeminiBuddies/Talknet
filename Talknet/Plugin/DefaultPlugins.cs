using System;
using Talknet.Plugin;

[assembly: TalknetPluginAssembly(typeof(DefaultPlugin), typeof(DefaultPlugin))]

namespace Talknet.Plugin {
    [TalknetPlugin("geminilab.default", "Default", "1.0.0.0", "Gemini Laboratory")]
    internal class DefaultPlugin : ITalknetPlugin {
        public void PluginFinalize() {
            // throw new NotImplementedException();
        }

        public void PluginInitialize() {
            // throw new NotImplementedException();
        }
    }
}
