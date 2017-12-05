using System;
using System.Collections.Generic;
using System.Text;

namespace Talknet.Plugin {
    public struct TalknetPluginInfo {
        public string Name;
        public string DisplayName;
        public Version Ver;
        public string Author;

        public override string ToString() => $"{DisplayName} - {Ver} - {Author}";
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class TalknetPluginAttribute : Attribute {
        internal TalknetPluginInfo Info { private set; get; }

        public TalknetPluginAttribute(string Name, string DisplayName, string VersionString, string Author) {
            Info = new TalknetPluginInfo { Name = Name, DisplayName = DisplayName, Ver = new Version(VersionString), Author = Author };
        }
    }

    [AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = false)]
    public sealed class TalknetPluginAssemblyAttribute: Attribute {
        public Type[] Plugins { private set; get; }
        public TalknetPluginAssemblyAttribute(params Type[] plugins) { Plugins = plugins; }
    }
}
