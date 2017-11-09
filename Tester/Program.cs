using System;
using System.Net;
using System.Threading;
using Talknet;

namespace Tester {
    class Program {
        static TalknetTcpClient client;

        static void E(object sender, TalknetTcpClientDataEventArgs e) {
            Console.WriteLine("E triggered");
            Thread.Sleep(2000);
            Console.Write(client.ReadAllAsString());
        }

        static void Main(string[] args) {
            client = new TalknetTcpClient();

            client.OnData += E;
            client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345));

            for (int i = 0; i < 100; ++i) {
                client.Send("ffffffffdeadbeef\n");
                Thread.Sleep(834);
            }

            client.Send("bye bye ~~");
            client.Disconnect();
        }
    }
}
