using System.Text;

namespace Talknet {
    internal static class Consts {
        public static readonly Encoding DefaultEncoding = Encoding.UTF8;

        public const int CheckIntervalMilli = 50;

        public const string NameOfNull = "null";
        public const string NameOfSelfAssembly = "<self>";
        public static readonly string[] AcceptableAssemblyExtensions = { "exe", "dll" };

        public const string PluginDirectoryName = "plugins";
        public const string PluginNamePattern = @"\A[a-zA-Z_][a-zA-Z0-9_]*(\.[a-zA-Z_][a-zA-Z0-9_]*)*\z";
    }
}
