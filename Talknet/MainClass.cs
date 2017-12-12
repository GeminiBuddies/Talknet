#if !COLORLESS
#define COLORFUL
#endif

using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using Talknet.i18n;
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
        static readonly string ipPortPattern = @"\A(?<ip>(?<ipa>\d{1,3})\.(?<ipb>\d{1,3})\.(?<ipc>\d{1,3})\.(?<ipd>\d{1,3})):(?<port>\d{1,5})\z";
        static readonly Regex ipPortRegex = new Regex(ipPortPattern);

        static bool connected { get => client?.Connected ?? false; }
        static string remote = "";
        static TalknetTcpClient client;

#if COLORLESS
        static string getPrompt() => (connected ? remote : "offline") + "> ";
        static void printPrompt() => Console.Write(getPrompt());

        static void printErrMsgLine(string msg) => Console.WriteLine(msg);

        static void printRemote(string msg) => Console.WriteLine(msg);
#else
        static string getPrompt() => (connected ? "@g" + remote + "@!" : "offline") + "> ";
        static void printPrompt() => Exconsole.Write(getPrompt());

        static void printErrMsgLine(string msg) => Exconsole.WriteLine("@r" + msg + "@!");

        static void printRemote(string msg) => Exconsole.WriteLine("@y" + msg + "@!");
#endif

        static void initialize() {
            refreshClient();
            
            _invoker = new CommandInvoker();
            _invoker.Register<IPEndPoint>("connect", Connect);
            _invoker.Register<IPEndPoint>("c", Connect);
            _invoker.Register("disconnect", Disconnect);
            _invoker.Register("d", Disconnect);
            _invoker.Register<string[]>("send", Send);
            _invoker.Register<string[]>("s", Send);
            _invoker.Register<string>("direct", DirectMode);
            _invoker.Register("dl", () => DirectMode(false));
            _invoker.Register("db", () => DirectMode(true));
            _invoker.Register("quit", Exit);
            _invoker.Register("exit", Exit);
            _invoker.Register("q", Exit);
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

        static bool _exiting;
        static CommandInvoker _invoker;
        public static void Main(string[] args) {
            initialize();

            _exiting = false;
            while (!_exiting) {
                printPrompt();

                var line = Console.ReadLine().Trim();
                if (line.Length == 0) continue;

                try {
                    // carl.InvokeFromLine(line);
                    _invoker.InvokeFromLine(line);
                } catch (CommandNotFoundException ex) {
                    printErrMsgLine(string.Format(ErrMsg.UnknownCommand, ex.Command));
                } catch (TalknetCommandException ex) {
                    printErrMsgLine(string.Format(ErrMsg.CommandExceptionDesc, ex.Message));

                    if (ex.InnerException == null) {
                        printErrMsgLine(ErrMsg.NoInnerException);
                    } else {
                        printErrMsgLine(string.Format(ErrMsg.InnerExceptionDesc, ex.InnerException.GetType().FullName, ex.InnerException.Message));
                        printErrMsgLine(ErrMsg.ExceptionStacktrace);
                        printErrMsgLine(ex.InnerException.StackTrace);
                    }
                } catch (Exception ex) {
                    printErrMsgLine(ErrMsg.FatalException);
                    printErrMsgLine(string.Format(ErrMsg.ExceptionDesc, ex.GetType().FullName, ex.Message));
                    printErrMsgLine(ErrMsg.ExceptionStacktrace);
                    printErrMsgLine(ex.StackTrace);

                    _exiting = true;
                    break;
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
            var matches = ipPortRegex.Matches(addr);
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
            if (connected) throw new InvalidOperationException(ErrMsg.AlreadyConnected);

            remote = addr.ToString();
            client.Connect(addr);
            return 0;
        }

        public static int Disconnect() {
            if (!connected) throw new InvalidOperationException(ErrMsg.NotConnected);

            client.Disconnect();
            refreshClient();
            return 0;
        }

        private static void refreshClient() {
            client = new TalknetTcpClient();
            client.OnData += (sender, e) => { printRemote(client.ReadAllAsString()); };
        }

        public static int Send(string cont) {
            if (!connected) throw new InvalidOperationException(ErrMsg.NotConnected);

            client.Send(cont);
            return 0;
        }

        public static int Send(params string[] cont) {
            foreach (var s in cont) Send(s);
            return 0;
        }

        public static int DirectMode(string mode) {
            if (mode == "block") return DirectMode(true);
            else if (mode == "line") return DirectMode(false);
            throw new ArgumentException();
        }

        public static int DirectMode(bool blockMode) {
            if (!connected) throw new InvalidOperationException(ErrMsg.NotConnected);

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
