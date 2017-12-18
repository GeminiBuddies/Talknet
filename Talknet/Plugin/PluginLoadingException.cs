using System;
using Talknet.i18n;

namespace Talknet.Plugin {
    internal enum CommonLoadingErrorTypes {
        AssemblyNoPluginList,
        PluginListNullItem,
        PluginNoInterface,
        PluginNoAttribute,
        PluginInvalidName
    }

    internal class PluginLoadingException : Exception {
        // public PluginLoadingException() { }
        public PluginLoadingException(string message) : base(message) { }

        public PluginLoadingException(string message, Exception inner) : base(message, inner) { }

        public PluginLoadingException(CommonLoadingErrorTypes error, string assemblyName, string itemName = null) : base(getMsg(error, assemblyName, itemName)) { }

        public static PluginLoadingException ErrorGettingAttr(string assemblyName, string itemName, Exception inner) {
            return new PluginLoadingException(string.Format(ErrMsg.ErrorGettingPluginAttribute, assemblyName, itemName), inner);
        }

        private static string getMsg(CommonLoadingErrorTypes error, string assemblyName, string itemName) {
            switch (error) {
            case CommonLoadingErrorTypes.AssemblyNoPluginList:
                return string.Format(ErrMsg.PluginAssemblyNoPluginList, assemblyName);
            case CommonLoadingErrorTypes.PluginListNullItem:
                return string.Format(ErrMsg.PluginListInvalidItem, assemblyName, Consts.NameOfNull, ErrMsg.PluginItemNull);
            case CommonLoadingErrorTypes.PluginNoInterface:
                return string.Format(ErrMsg.PluginListInvalidItem, assemblyName, itemName, ErrMsg.PluginItemNoInterface);
            case CommonLoadingErrorTypes.PluginNoAttribute:
                return string.Format(ErrMsg.PluginListInvalidItem, assemblyName, itemName, ErrMsg.PluginItemNoAttribute);
            case CommonLoadingErrorTypes.PluginInvalidName:
                return string.Format(ErrMsg.PluginListInvalidItem, assemblyName, itemName, ErrMsg.PluginItemInvalidName);
            default:
                throw new ArgumentOutOfRangeException(nameof(error), error, null);
            }
        }
    }
}