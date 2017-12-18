using System;
using Talknet.i18n;
using Talknet.Invoker;
using Talknet.Plugin;
using System.Linq;

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
        private const string PluginLoaderName = "Plugin Loader";

        private static bool Connected => _client?.Connected ?? false;
        private static string _remote = "";
        private static TalknetTcpClient _client;

        private static string getPrompt() => (Connected ? "@g" + _remote + "@!" : "offline") + "> ";
        private static void printPrompt() => Logger.Write(getPrompt());

        private static bool initialize() {
            _client = new TalknetTcpClient();

            _invoker = new CommandInvoker();
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
            _invoker.Register("dp", () => {
                Logger.WriteLine(
                    PluginManager.Plugins.Select(p => $"{p.Key}: {p.Value.Info.ToString()}({p.Value.Source})").JoinBy(Environment.NewLine)
                );
                return 0;
            }, "displayplugins");

            try {
                PluginManager.InitializeManager();
                PluginManager.LoadAndInitializePlugins(new TalknetEnv {
                    RemoteSetter = str => _remote = str,
                    Client = _client,
                    Invoker = _invoker,
                    Exiter = () => _exiting = true
                });
            } catch (PluginLoadingException ex) {
                Logger.ErrorMultilineWithCaller(PluginLoaderName,
                    string.Format(ErrMsg.PluginLoadingException, ex.Message),
                    ErrMsg.ExceptionStacktrace,
                    ex.StackTrace
                );

                if (ex.InnerException is Exception ine) {
                    Logger.ErrorMultilineWithCaller(PluginLoaderName,
                        string.Format(ErrMsg.InnerExceptionDesc, ine.GetType().FullName, ine.Message),
                        ErrMsg.ExceptionStacktrace,
                        ine.StackTrace
                    );
                }

                return false;
            }

            return true;
        }

        private static bool _exiting;
        private static CommandInvoker _invoker;

        public static void Main(string[] args) {
            if (!initialize()) return;

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

            PluginManager.FinalizePlugins();
            if (Connected) _client.Disconnect();
        }
    }
}
