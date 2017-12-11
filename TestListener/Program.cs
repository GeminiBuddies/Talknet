using System;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace TestListener {
    static class Program {
        static Socket _sock;
        static bool _exiting;

        public static int Main(string[] args) {
            if (args.Length == 0 || !int.TryParse(args[0], out int port)) {
                string p;

                do {
                    Console.Write("Port: ");
                    p = Console.ReadLine();
                } while (!int.TryParse(p, out port));
            }

            Console.WriteLine($"Listen on port {port}");

            var listener = new TcpListener(IPAddress.Any, port);
            listener.Start();

            _sock = listener.AcceptSocket();

            new Thread(receiver).Start();

            for (; ; ) {
                string v;
                if ((v = Console.ReadLine()) == null) break;

                _sock.Send(Encoding.UTF8.GetBytes(v));
            }

            _exiting = true;
            return 0;
        }

        static void receiver() {
            _exiting = false;
            while (!_exiting) {
                if (_sock.Available > 0) {
                    var length = _sock.Available;
                    var buffer = new byte[length];

                    _sock.Receive(buffer);
                    string data = Encoding.UTF8.GetString(buffer);

                    Console.Write(data);
                } else {
                    Thread.Sleep(50);
                }
            }

            _sock.Close();
        }
    }
}
