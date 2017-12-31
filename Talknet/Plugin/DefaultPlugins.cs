using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using Talknet.i18n;
using Talknet.Invoker;
using Talknet.Plugin;

[assembly: TalknetPluginAssembly(typeof(DefaultPlugin), typeof(DnsPlugin))]

namespace Talknet.Plugin {
    [TalknetPlugin("geminilab.default", "Default", "1.0.0.0", "Gemini Laboratory")]
    internal class DefaultPlugin : ITalknetPlugin {
        private static readonly string _ipPortPattern = @"\A(?<ip>(?<ipa>\d{1,3})\.(?<ipb>\d{1,3})\.(?<ipc>\d{1,3})\.(?<ipd>\d{1,3})):(?<port>\d{1,5})\z";
        private static readonly Regex _ipPortRegex = new Regex(_ipPortPattern);

        private CommandInvoker _invoker;
        private TalknetTcpClient _client;
        private Action<string> _remoteSetter;
        private Action _exiter;

        private bool Connected => _client?.Connected ?? false;

        public void PluginInitialize(TalknetEnv env) {
            _invoker = env.Invoker;
            _client = env.Client;
            _remoteSetter = env.RemoteSetter;
            _exiter = env.Exiter;

            _client.OnData += (sender, e) => { Logger.WriteHighlight(_client.ReadAllAsString()); };

            _invoker.Register<IPEndPoint>("c", Connect, "connect");
            _invoker.Register("d", Disconnect, "disconnect");
            _invoker.Register<string[]>("s", Send, "send");
            _invoker.Register<string>("direct", DirectMode);
            _invoker.Register("dl", () => DirectMode(false));
            _invoker.Register("db", () => DirectMode(true));
            _invoker.Register("q", Exit, "quit", "exit");

            _invoker.CustomParser(typeof(IPEndPoint), ParseIpEndPoint);
        }


        public int Exit() {
            _exiter();
            return 0;
        }

        internal static IPEndPoint ParseIpEndPoint(string addr) {
            var matches = _ipPortRegex.Matches(addr);
            Match match = null;
            bool valid = false;
            int port;
            string ip;

            if (matches.Count == 1) {
                match = matches[0];

                if (new[] { "ipa", "ipb", "ipc", "ipd" }.All(str => Ext.IsValidInteger(match.Groups[str].Value, out int x) && x >= 0 && x <= 255)) {
                    if (Ext.IsValidInteger(match.Groups["port"].Value, out port) && port > 0 && port < 65536) {
                        valid = true;
                    }
                }
            }

            if (!valid) throw new ArgumentException();

            ip = match.Groups["ip"].Value;
            port = int.Parse(match.Groups["port"].Value);

            return new IPEndPoint(IPAddress.Parse(ip), port);
        }

        public int Connect(IPEndPoint addr) {
            if (Connected) {
                Logger.Error(ErrMsg.AlreadyConnected);
                return 1;
            }

            _remoteSetter(addr.ToString());
            _client.Connect(addr);
            return 0;
        }

        public int Disconnect() {
            if (!Connected) {
                Logger.Error(ErrMsg.NotConnected);
                return 1;
            }

            _client.Disconnect();
            return 0;
        }

        public int Send(string cont) {
            if (!Connected) {
                Logger.Error(ErrMsg.NotConnected);
                return 1;
            }

            _client.Send(cont);
            return 0;
        }

        public int Send(params string[] cont) {
            var cache = 0;
            return cont.Any(s => (cache = Send(s)) != 0) ? cache : 0;
        }

        public int DirectMode(string mode) {
            if (mode == "block") return DirectMode(true);
            if (mode == "line") return DirectMode(false);

            Logger.Error($"\"{mode}\" is not a valid parameter.");
            return 1;
        }

        public int DirectMode(bool blockMode) {
            if (!Connected) {
                Logger.Error(ErrMsg.NotConnected);
                return 1;
            }

            var nl = Environment.NewLine;
            bool removeReturn = nl.Length == 2, replaceReturn = nl == "\r";
            var sb = new StringBuilder();

            int c;
            while ((c = Console.Read()) != -1) {
                // if (c == '\u001a') break;
                if (removeReturn && c == '\r') continue;
                if (replaceReturn && c == '\r') c = '\n';
                sb.Append((char)c);

                if (blockMode || c != '\n') continue;

                Send(sb.ToString());
                sb.Clear();
            }

            if (sb.Length != 0) Send(sb.ToString());

            return 0;
        }

        public void PluginFinalize() { }
    }

    [TalknetPlugin("geminilab.dns", "Dns resolver", "1.0.0.0", "Gemini Laboratory")]
    [Require("geminilab.default", LoadOrderType.RequiredFirst)]
    internal class DnsPlugin : ITalknetPlugin {
        public void PluginFinalize() { }

        public void PluginInitialize(TalknetEnv env) {
            env.Invoker.Register("dns",
                (string domain) => {
                    Logger.WriteLine(
                        Dns.GetHostAddresses(domain)
                           .Select(addr => addr.ToString())
                           .JoinBy(Environment.NewLine)
                    );
                    return 0;
                }
            );

            env.Invoker.CustomParser(typeof(IPEndPoint), ParseIpEndPointWithDns);
        }

        private static readonly string _domainPattern = @"(?=^.{4,253}$)(^(?<domain>((?!-)[a-zA-Z0-9-]{1,63}(?<!-)\.)+[a-zA-Z]{2,63}):(?<port>\d{1,5})$)";
        private static readonly Regex _domainRegex = new Regex(_domainPattern);

        public IPEndPoint ParseIpEndPointWithDns(string addr) {
            try {
                return DefaultPlugin.ParseIpEndPoint(addr);
            } catch (ArgumentException) {
                if (!_domainRegex.IsMatch(addr)) throw new ArgumentException();

                try {
                    var match = _domainRegex.Match(addr);

                    if (!Ext.IsValidInteger(match.Groups["port"].Value, out var port)
                     || port <= 0 || port >= 65536
                     || !(Dns.GetHostAddresses(match.Groups["domain"].Value).FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork) is IPAddress ipaddr))
                        throw new ArgumentException();

                    return new IPEndPoint(ipaddr, port);
                } catch (Exception) {
                    throw new ArgumentException();
                }
            }
        }
    }
}