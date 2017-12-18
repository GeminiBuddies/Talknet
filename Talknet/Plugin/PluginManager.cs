using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Talknet.i18n;
using Talknet.Plugin.DAG;

namespace Talknet.Plugin {
    internal static class PluginManager {
        internal static Dictionary<string, LoadedPlugin> Plugins { get; private set; }
        private static IEnumerable<string> _order;

        internal static void InitializeManager() {
            Plugins = new Dictionary<string, LoadedPlugin>();
        }

        internal static void LoadAndInitializePlugins(TalknetEnv env) {
            var pluginsCache = new List<LoadedPlugin>();

            pluginsCache.AddRange(LoadSelf());
            pluginsCache.AddRange(LoadFromPluginDirectory());

            // unique
            foreach (var plugin in pluginsCache) {
                var name = plugin.Info.Name;

                if (Plugins.ContainsKey(name)) {
                    if (plugin.PluginInstance.GetType().IsInstanceOfType(Plugins[name].PluginInstance)) continue;
                    throw new PluginLoadingException(string.Format(ErrMsg.PluginSameName, name, Plugins[name], plugin));
                }

                Plugins[name] = plugin;
            }

            // check requirements
            var refs = new DAG<string>(Plugins.Keys);

            foreach (var plugin in pluginsCache) {
                var thisName = plugin.Info.Name;

                foreach (var req in plugin.Requirements) {
                    var reqName = req.Requirement;

                    if (!Plugins.ContainsKey(reqName)) throw PluginLoadingException.ErrorReqNotSatisfied(plugin, reqName);

                    if (req.Order != LoadOrderType.Any) {
                        if (req.Order == LoadOrderType.ThisFirst) {
                            refs.AddEdge(thisName, reqName);
                        } else {
                            refs.AddEdge(reqName, thisName);
                        }
                    }
                }
            }

            _order = null;
            try {
                _order = refs.TopologicalOrder();
            } catch (NotADagException) {
                throw new Exception();
            }
            
            foreach (var it in _order) {
                Plugins[it].PluginInstance.PluginInitialize(env);
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

        internal static void FinalizePlugins() {
            foreach (var it in _order.Reverse()) {
                Plugins[it].PluginInstance.PluginFinalize();
            }
        }
    }
}
