using System;

namespace Talknet.Plugin {
    public struct TalknetPluginInfo {
        public string Name;
        public string DisplayName;
        public Version Ver;
        public string Author;

        public override string ToString() => $"{DisplayName} - {Ver} - {Author}";
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class TalknetPluginAttribute : Attribute {
        internal TalknetPluginInfo Info { get; }

        public TalknetPluginAttribute(string name, string displayName, string versionString, string author) {
            Info = new TalknetPluginInfo { Name = name, DisplayName = displayName, Ver = new Version(versionString), Author = author };
        }

        public TalknetPluginAttribute(TalknetPluginInfo info) {
            Info = info;
        }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class RequireAttribute : Attribute {
        internal string Requirement { get; }
        internal LoadOrderType Order { get; }

        public RequireAttribute(string requirement, LoadOrderType order) {
            Requirement = requirement;
            Order = order;
        }
    }

    public enum LoadOrderType {
        ThisFirst,
        RequiredFirst,
        Any
    }

    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class TalknetPluginAssemblyAttribute : Attribute {
        public Type[] Plugins { get; }
        public TalknetPluginAssemblyAttribute(params Type[] plugins) { Plugins = plugins; }
    }
}
