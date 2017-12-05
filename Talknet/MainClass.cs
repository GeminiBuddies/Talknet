#if !COLORLESS
#define COLORFUL
#endif

using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
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
        internal static bool isValidInt(string v, out int x) {
            x = 0;
            if (int.TryParse(v, out int y)) {
                if (y != 0 && v[0] == '0') return false;
                x = y;
                return true;
            }

            return false;
        }

        static readonly string ipPortPattern = @"\A(?<ip>(?<ipa>\d{1,3})\.(?<ipb>\d{1,3})\.(?<ipc>\d{1,3})\.(?<ipd>\d{1,3})):(?<port>\d{1,5})\z";
        static readonly Regex ipPortRegex = new Regex(ipPortPattern);

        static bool connected { get => client?.Connected ?? false; }
        static string remote = "";
        static TalknetTcpClient client;

#if COLORLESS
        static string getPrompt() => (connected ? remote : "offline") + "> ";
        static void PrintPrompt() => Console.Write(getPrompt());

        static void PrintErrMsgLine(string msg) => Console.WriteLine(msg);

        static void PrintRemote(string msg) => Console.WriteLine(msg);
#else
        static string getPrompt() => (connected ? "@g" + remote + "@!" : "offline") + "> ";
        static void PrintPrompt() => Exconsole.Write(getPrompt());

        static void PrintErrMsgLine(string msg) => Exconsole.WriteLine("@r" + msg + "@!");

        static void PrintRemote(string msg) => Exconsole.WriteLine("@y" + msg + "@!");
#endif

        static void initialize() {
            refreshClient();

            carl = new CommandInvoker<int>()
                .RegisterNew("connect", connect)
                .RegisterNew("c", connect)
                .RegisterNew("disconnect", disconnect)
                .RegisterNew("d", disconnect)
                .RegisterNew("send", send)
                .RegisterNew("s", send)
                .RegisterNew("direct", direct)
                .RegisterNew("dl", dl)
                .RegisterNew("db", db)
                .RegisterNew("quit", exit)
                .RegisterNew("exit", exit)
                .RegisterNew("q", exit)
#if DEBUG
                .RegisterNew("generr", (any, thing) => { try { throw new Exception("efi kwo semé"); } catch (Exception ex) { throw new TalknetCommandException("um-cyroga", ex); } })
                .RegisterNew("genfatal", (any, thing) => throw new Exception("il-pavoka"))
#endif
            ;

            PluginManager.SelfLoad();
        }

        static bool exiting;
        static CommandInvoker<int> carl;
        public static void Main(string[] args) {
            initialize();

            exiting = false;
            while (!exiting) {
                PrintPrompt();

                var line = Console.ReadLine().Trim();
                if (line.Length == 0) continue;

                try {
                    carl.InvokeFromLine(line);
                } catch (CommandNotFoundException ex) {
                    PrintErrMsgLine(string.Format(ErrMsg.UnknownCommand, ex.Command));
                } catch (CommandArgumentException ex) {
                    PrintErrMsgLine($"{ex.Command}: {ex.Description}");
                } catch (TalknetCommandException ex) {
                    PrintErrMsgLine(string.Format(ErrMsg.CommandExceptionDesc, ex.Message));

                    if (ex.InnerException == null) {
                        PrintErrMsgLine(ErrMsg.NoInnerException);
                    } else {
                        PrintErrMsgLine(string.Format(ErrMsg.InnerExceptionDesc, ex.InnerException.GetType().FullName, ex.InnerException.Message));
                        PrintErrMsgLine(ErrMsg.ExceptionStacktrace);
                        PrintErrMsgLine(ex.InnerException.StackTrace);
                    }
                } catch (Exception ex) {
                    PrintErrMsgLine(ErrMsg.FatalException);
                    PrintErrMsgLine(string.Format(ErrMsg.ExceptionDesc, ex.GetType().FullName, ex.Message));
                    PrintErrMsgLine(ErrMsg.ExceptionStacktrace);
                    PrintErrMsgLine(ex.StackTrace);

                    exiting = true;
                    break;
                }
            }

            if (connected) Disconnect();
        }

        // Command handlers
        private static int exit(string command, string[] args) {
            if (args.Length != 0) throw new CommandArgumentException(command, "No argument expected.");

            exiting = true;
            return 0;
        }

        private static int connect(string command, string[] args) {
            if (args.Length != 1) throw new CommandArgumentException(command, "Expected exact 1 argument.");

            string addr = args[0];
            var matches = ipPortRegex.Matches(addr);
            Match match = null;
            bool valid = false;
            int port; string ip;

            if (matches.Count == 1) {
                match = matches[0];

                if (new string[] { "ipa", "ipb", "ipc", "ipd" }.All(str => isValidInt(match.Groups[str].Value, out int x) && x >= 0 && x <= 255)) {
                    if (isValidInt(match.Groups["port"].Value, out port) && port > 0 && port < 65536) {
                        valid = true;
                    }
                }
            }

            if (!valid) throw new CommandArgumentException(command, $"Invalid address: \"{addr}\".");

             ip = match.Groups["ip"].Value;
             port = int.Parse(match.Groups["port"].Value);

            remote = match.Value;

            return Connect(new IPEndPoint(IPAddress.Parse(ip), port));
        }

        private static int disconnect(string command, string[] args) {
            if (args.Length != 0) throw new CommandArgumentException(command, "No argument expected.");

            Disconnect();
            return 0;
        }

        private static int send(string command, string[] args) {
            if (args.Length == 0) throw new CommandArgumentException(command, "At least 1 argument expected.");

            foreach (var i in args) Send(i);
            return 0;
        }

        private static int direct(string command, string[] args) {
            if (args.Length != 1) throw new CommandArgumentException(command, "Expected exact 1 argument.");

            if (args[0] == "line") return DirectMode(false);
            else if (args[0] == "block") return DirectMode(true);
            else throw new CommandArgumentException(command, $"Unexpected argument: \"{args[0]}\".");
        }

        private static int dl(string command, string[] args) {
            if (args.Length != 0) throw new CommandArgumentException(command, ErrMsg.ExpectedNoArg);
            return DirectMode(false);
        }

        private static int db(string command, string[] args) {
            if (args.Length != 0) throw new CommandArgumentException(command, ErrMsg.ExpectedNoArg);
            return DirectMode(true);
        }

        public static int Connect(IPEndPoint addr) {
            if (connected) throw new InvalidOperationException("Already connected.");

            client.Connect(addr);
            return 0;
        }

        public static int Disconnect() {
            if (!connected) throw new InvalidOperationException("Not connected yet.");

            client.Disconnect();
            refreshClient();
            return 0;
        }

        private static void refreshClient() {
            client = new TalknetTcpClient();
            client.OnData += (sender, e) => { PrintRemote(client.ReadAllAsString()); };
        }

        public static int Send(string cont) {
            if (!connected) throw new InvalidOperationException("Not connected yet.");

            client.Send(cont);
            return 0;
        }

        public static int DirectMode(bool blockMode) {
            if (!connected) throw new InvalidOperationException("Not connected yet.");

            string nl = System.Environment.NewLine;
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
