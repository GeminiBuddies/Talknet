using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Talknet.Plugin {
    internal static class PluginManager {
        internal static Dictionary<string, LoadedPlugin> Plugins { get; private set; }

        internal static void InitializeManager() {
            Plugins = new Dictionary<string, LoadedPlugin>();
        }

        internal static void LoadAndInitializePlugins() {
            var pluginsCache = new List<LoadedPlugin>();

            pluginsCache.AddRange(LoadSelf());
            pluginsCache.AddRange(LoadFromPluginDirectory());

            // unique
            foreach (var plugin in pluginsCache) {
                var name = plugin.Info.Name;

                if (Plugins.ContainsKey(name)) {
                    if (plugin.PluginInstance.GetType().IsInstanceOfType(Plugins[name].PluginInstance)) continue;
                    throw new PluginLoadingException($"Two plugins with a same name.\n{Plugins[name]} and {plugin}.");
                }

                Plugins[name] = plugin;
            }

            foreach (var it in Plugins.Values) {
                it.PluginInstance.PluginInitialize();
            }
        }

        internal static IEnumerable<LoadedPlugin> LoadSelf() {
            return PluginLoader.LoadFromAssembly(Assembly.GetExecutingAssembly(), Consts.NameOfSelfAssembly);
        }

        internal static IEnumerable<LoadedPlugin> LoadFromPluginDirectory() {
            var ps = Path.DirectorySeparatorChar;
            var pluginDir = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory + ps, "." + ps + Consts.PluginDirectoryName + ps));

            return pluginDir.Exists ? PluginLoader.LoadAllFromDirectory(pluginDir) : new LoadedPlugin[0];
        }
    }
}
