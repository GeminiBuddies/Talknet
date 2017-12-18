namespace Talknet.Plugin {
    internal struct LoadedPlugin {
        public TalknetPluginInfo Info;
        public ITalknetPlugin PluginInstance;
        public string Source;
        public RequireAttribute[] Requirements;

        public override string ToString() => $"{Info} ({Source})";
    }
}
