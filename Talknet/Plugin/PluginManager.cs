using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Talknet.i18n;

namespace Talknet.Plugin {

    internal static class PluginManager {
        

        internal static void SelfLoad() {
            var a = PluginLoader.LoadFromAssembly(Assembly.GetExecutingAssembly(), "<self>");
        }
    }
}
