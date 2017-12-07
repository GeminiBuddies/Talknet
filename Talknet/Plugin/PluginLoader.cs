using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Clet = Talknet.Plugin.CommonLoadingErrorTypes;

namespace Talknet.Plugin {
    internal class PluginLoader {
        private static readonly Regex pluginNameRegex = new Regex(Consts.PluginNamePattern);
        
        // do not handle exceptions, just throw them
        internal static IEnumerable<LoadedPlugin> LoadFromAssembly(Assembly ass, string from) {
            var cache = new List<LoadedPlugin>();

            TalknetPluginAssemblyAttribute attrAssembly;
            if ((attrAssembly = ass.GetCustomAttribute(typeof(TalknetPluginAssemblyAttribute)) as TalknetPluginAssemblyAttribute) == null) return new LoadedPlugin[0];

            var pluginsProvided = attrAssembly.Plugins;
            if (pluginsProvided == null || pluginsProvided.Length == 0) throw new PluginLoadingException(Clet.AssemblyNoPluginList, from);

            foreach (var pluginType in pluginsProvided) {
                if (pluginType == null) throw new PluginLoadingException(Clet.PluginListNullItem, from);

                if (!pluginType.GetInterfaces().Contains(typeof(ITalknetPlugin))) throw new PluginLoadingException(Clet.PluginNoInterface, from, pluginType.FullName);
                if (!(pluginType.GetCustomAttribute(typeof(TalknetPluginAttribute)) is TalknetPluginAttribute attr)) throw new PluginLoadingException(Clet.PluginNoAttribute, from, pluginType.FullName);

                // so it is
                var info = attr.Info;
                var ins = Activator.CreateInstance(pluginType) as ITalknetPlugin;

                if (!pluginNameRegex.IsMatch(info.Name)) throw new PluginLoadingException(Clet.PluginInvalidName, from, pluginType.FullName);

                cache.Add(new LoadedPlugin { Info = info, PluginInstance = ins, Source = from });
            }

            return cache;
        }

        // do not throw if it's not a valid .net assembly 
        // just return an empty array
        internal static IEnumerable<LoadedPlugin> LoadFromFile(FileInfo file) {
            var ass = Assembly.LoadFile(file.FullName);
            
            if (!(ass.GetCustomAttribute(typeof(TalknetPluginAssemblyAttribute)) is TalknetPluginAssemblyAttribute)) return new LoadedPlugin[0];

            return LoadFromAssembly(ass, file.FullName);
        }

        internal static IEnumerable<LoadedPlugin> LoadAllFromDirectory(DirectoryInfo directory) {
            var cache = new List<LoadedPlugin>();

            foreach (var file in directory.GetFiles()) {
                if (!Consts.AcceptableAssemblyExtensions.Contains(file.Extension)) continue;

                try {
                    cache.AddRange(LoadFromFile(file));
                } catch (PluginLoadingException) {
                }
            }

            return cache;
        }
    }
}
