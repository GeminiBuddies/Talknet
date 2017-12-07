namespace Talknet.Plugin {
    /// <summary>Don't forget to use TalknetPluginAttribute when implement this interface.</summary>
    public interface ITalknetPlugin {
        void PluginInitialize();
        void PluginFinalize();
    }
}
