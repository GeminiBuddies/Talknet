using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using Talknet.i18n;
using Talknet.Invoker;
using Talknet.Plugin;

/*
 * Prompt :
 * serverIp:port|"offline">
 * 
 * Command :
 * connect | c  server:ip
 * disconnect | d
 * send | s [content]
 *     can receive multiple args
 * direct line | dl
 * direct block | db
 */


namespace Talknet {
    public static class MainClass {
        private const string MainLoopName = "Main Loop";

        private static readonly string _ipPortPattern = @"\A(?<ip>(?<ipa>\d{1,3})\.(?<ipb>\d{1,3})\.(?<ipc>\d{1,3})\.(?<ipd>\d{1,3})):(?<port>\d{1,5})\z";
        private static readonly Regex _ipPortRegex = new Regex(_ipPortPattern);

        private static bool connected { get => client?.Connected ?? false; }
        private static string remote = "";
        private static TalknetTcpClient client;
        
        private static string getPrompt() => (connected ? "@g" + remote + "@!" : "offline") + "> ";
        private static void printPrompt() => Logger.Write(getPrompt());

        private static void printRemote(string msg) => Logger.WriteHighlight(msg);

        private static void initialize() {
            refreshClient();

            _invoker = new CommandInvoker();
            _invoker.Register<IPEndPoint>("c", Connect, "connect");
            _invoker.Register("d", Disconnect, "disconnect");
            _invoker.Register<string[]>("s", Send, "send");
            _invoker.Register<string>("direct", DirectMode);
            _invoker.Register("dl", () => DirectMode(false));
            _invoker.Register("db", () => DirectMode(true));
            _invoker.Register("q", Exit, "quit", "exit");
#if DEBUG
            _invoker.Register("generr", () => {
                try {
                    throw new Exception("efi kwo semé");
                } catch (Exception ex) {
                    throw new TalknetCommandException("um-cyroga", ex);
                }
            });
            _invoker.Register("genfatal", () => throw new Exception("il-pavoka"));
#endif

            _invoker.CustomParser(typeof(IPEndPoint), parseIpEndPoint);

            PluginManager.InitializeManager();
            PluginManager.LoadAndInitializePlugins();
        }

        private static bool _exiting;
        private static CommandInvoker _invoker;
        public static void Main(string[] args) {
            initialize();

            _exiting = false;
            while (!_exiting) {
                printPrompt();

                var line = Console.ReadLine().Trim();
                if (line.Length == 0) continue;

                try {
                    _invoker.InvokeFromLine(line);
                } catch (CommandNotFoundException ex) {
                    Logger.Error(string.Format(ErrMsg.UnknownCommand, ex.Command), MainLoopName);
                } catch (CommandExitAbnormallyException ex) {
                    var innerException = ex.InnerException;
                    Logger.ErrorMultilineWithCaller(MainLoopName,
                        ErrMsg.FatalException,
                        string.Format(ErrMsg.ExceptionDesc, innerException.GetType().FullName, innerException.Message),
                        ErrMsg.ExceptionStacktrace,
                        innerException.StackTrace
                    );

                    _exiting = true;
                    break;
                } catch (ArgumentParsingException ex) {
                    var innerException = ex.InnerException;

                    Logger.ErrorMultilineWithCaller(MainLoopName,
                        ex.Message,
                        string.Format(ErrMsg.ExceptionDesc, innerException.GetType().FullName, innerException.Message),
                        ErrMsg.ExceptionStacktrace,
                        innerException.StackTrace
                    );
                } catch (CommandArgumentCountException ex) {
                    Logger.Error(ex.Message, MainLoopName);
                } catch (DoNotKnowHowToParseException ex) {
                    Logger.Error(ex.Message, MainLoopName);
                } catch (TalknetCommandException ex) {
                    Logger.Error(string.Format(ErrMsg.CommandExceptionDesc, ex.Message), MainLoopName);

                    if (ex.InnerException is Exception ine) {
                        Logger.ErrorMultilineWithCaller(MainLoopName,
                            string.Format(ErrMsg.InnerExceptionDesc, ine.GetType().FullName, ine.Message),
                            ErrMsg.ExceptionStacktrace,
                            ine.StackTrace
                        );
                    } else {
                        Logger.Error(ErrMsg.NoInnerException, MainLoopName);
                    }
                }
            }

            if (connected) Disconnect();
        }

        // Command handlers
        public static int Exit() {
            _exiting = true;
            return 0;
        }

        private static IPEndPoint parseIpEndPoint(string addr) {
            var matches = _ipPortRegex.Matches(addr);
            Match match = null;
            bool valid = false;
            int port; string ip;

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

        public static int Connect(IPEndPoint addr) {
            if (connected) { Logger.Error(ErrMsg.AlreadyConnected); return 1; }

            remote = addr.ToString();
            client.Connect(addr);
            return 0;
        }

        public static int Disconnect() {
            if (!connected) { Logger.Error(ErrMsg.NotConnected); return 1; }

            client.Disconnect();
            refreshClient();
            return 0;
        }

        private static void refreshClient() {
            client = new TalknetTcpClient();
            client.OnData += (sender, e) => { printRemote(client.ReadAllAsString()); };
        }

        public static int Send(string cont) {
            if (!connected) { Logger.Error(ErrMsg.NotConnected); return 1; }

            client.Send(cont);
            return 0;
        }

        public static int Send(params string[] cont) {
            var cache = 0;
            return cont.Any(s => (cache = Send(s)) != 0) ? cache : 0;
        }

        public static int DirectMode(string mode) {
            if (mode == "block") return DirectMode(true);
            if (mode == "line") return DirectMode(false);

            Logger.Error($"\"{mode}\" is not a valid parameter.");
            return 1;
        }

        public static int DirectMode(bool blockMode) {
            if (!connected) { Logger.Error(ErrMsg.NotConnected); return 1; }

            string nl = Environment.NewLine;
            bool removeReturn = nl.Length == 2, replaceReturn = nl == "\r";
            StringBuilder sb = new StringBuilder();

            int c;
            while ((c = Console.Read()) != -1) {
                // if (c == '\u001a') break;
                if (removeReturn && c == '\r') continue;
                if (replaceReturn && c == '\r') c = '\n';
                sb.Append((char)c);

                if (!blockMode && c == '\n') {
                    Send(sb.ToString());
                    sb.Clear();
                }
            }

            if (sb.Length != 0) Send(sb.ToString());

            return 0;
        }
    }
}
