namespace Talknet.Plugin {
    public struct LoadedPlugin {
        public TalknetPluginInfo Info;
        public ITalknetPlugin PluginInstance;
        public string Source;

        public override string ToString() => $"{Info} ({Source})";
    }
}
