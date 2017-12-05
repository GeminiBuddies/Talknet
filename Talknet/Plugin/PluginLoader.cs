using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Talknet.i18n;

namespace Talknet.Plugin {
    public struct LoadedPlugin {
        public TalknetPluginInfo Info;
        public ITalknetPlugin PluginInstance;
    }

    [Serializable]
    public class PluginLoadingException : Exception {
        // public PluginLoadingException() { }
        public PluginLoadingException(string message) : base(message) { }
        public PluginLoadingException(string message, Exception inner) : base(message, inner) { }
    }

    internal class PluginLoader {
        private static void throwInvalidPluginListException(string path, string itemName) {
            throw new PluginLoadingException(string.Format(ErrMsg.PluginAssemblyInvalidPluginList, path, itemName));
        }

        // do not handle exceptions, just throw them
        internal static IEnumerable<LoadedPlugin> LoadFromAssembly(Assembly ass, string from) {
            List<LoadedPlugin> cache = new List<LoadedPlugin>();

            TalknetPluginAssemblyAttribute attr;
            if ((attr = ass.GetCustomAttribute(typeof(TalknetPluginAssemblyAttribute)) as TalknetPluginAssemblyAttribute) == null) return new LoadedPlugin[0];

            var pluginsProvided = attr.Plugins;
            if (pluginsProvided == null || pluginsProvided.Length == 0) throwInvalidPluginListException(from, "null");

            foreach (var pluginType in pluginsProvided) {
                if (pluginType == null) throwInvalidPluginListException(from, "null");

                if (!pluginType.GetInterfaces().Contains(typeof(ITalknetPlugin))) throwInvalidPluginListException(from, pluginType.FullName);
                if (pluginType.GetCustomAttribute(typeof(TalknetPluginAttribute)) == null) throwInvalidPluginListException(from, pluginType.FullName);

                // so it is
                var info = (pluginType.GetCustomAttribute(typeof(TalknetPluginAttribute)) as TalknetPluginAttribute).Info;
                var ins = Activator.CreateInstance(pluginType) as ITalknetPlugin;

                cache.Add(new LoadedPlugin { Info = info, PluginInstance = ins });
            }

            return cache;
        }

        // do not throw if it's not a valid .net assembly 
        // just return an empty array
        internal static IEnumerable<LoadedPlugin> LoadFromFile(FileInfo file) {
            var ass = Assembly.LoadFile(file.FullName);

            TalknetPluginAssemblyAttribute attr;
            if ((attr = ass.GetCustomAttribute(typeof(TalknetPluginAssemblyAttribute)) as TalknetPluginAssemblyAttribute) == null) return new LoadedPlugin[0];

            return LoadFromAssembly(ass, file.FullName);
        }

        internal static string[] AcceptableExtensions = { "exe", "dll" };
        internal static IEnumerable<LoadedPlugin> LoadAllFromDirectory(DirectoryInfo directory) {
            List<LoadedPlugin> cache = new List<LoadedPlugin>();

            foreach (var file in directory.GetFiles()) {
                if (!AcceptableExtensions.Contains(file.Extension)) continue;

                try {
                    cache.AddRange(LoadFromFile(file));
                } catch (PluginLoadingException ex) {
                    continue;
                } catch (Exception) {
                    throw;
                }
            }

            return cache;
        }
    }
}
