using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Talknet.i18n;

namespace Talknet {
    public class TalknetTcpClientDataEventArgs : EventArgs { }
    public delegate void TalknetTcpClientDataEventHandler(object sender, TalknetTcpClientDataEventArgs e);

    /// <summary>Only ipv4 now.</summary>
    public class TalknetTcpClient {
        // private member
        TcpClient client = null;
        List<byte> recvCache = null;

        // properties
        public Encoding DefaultEncoding { get; set; } = Consts.DefaultEncoding;
        public bool Connected { get => client != null; }
        public int BytesAvailable { get => Connected ? client.Client.Available : throw new InvalidOperationException(ErrMsg.NotConnected); }

        // event
        public event TalknetTcpClientDataEventHandler OnData;

        // send
        public void Send(string str, Encoding encoding = null) {
            encoding = encoding ?? DefaultEncoding;

            Send(encoding.GetBytes(str));
        }

        public void Send(byte[] data) {
            if (client == null) throw new InvalidOperationException(ErrMsg.NotConnected);
            if (data.Length == 0) throw new ArgumentOutOfRangeException(nameof(data), string.Format(ErrMsg.EmptyArg, nameof(data)));

            client.Client.Send(data);
        }

        public async Task SendAsync(string str, Encoding encoding = null) {
            await Task.Run(() => { Send((encoding ?? DefaultEncoding).GetBytes(str)); });
        }

        public async Task SendAsync(byte[] data) {
            await Task.Run(() => { Send(data); });
        }

        // receive
        private void beginReceive() {
            receiving = true;
        }

        private void endReceive() {
            receiving = false;
        }

        bool receiving = false;
        protected virtual void receiver() {
            recvCache = new List<byte>();
            while (!receiving) Thread.Sleep(Consts.CheckIntervalMilli);

            while (receiving) {
                if (client.Available == 0) { Thread.Sleep(Consts.CheckIntervalMilli); continue; }

                bool invoking = recvCache.Count == 0;

                int count = client.Available;
                var cache = new byte[count];
                client.Client.Receive(cache, count, SocketFlags.None);
                recvCache.AddRange(cache); // note that List.AddRange will treat byte[] as ICollection<byte>

                if (invoking) OnData?.Invoke(this, new TalknetTcpClientDataEventArgs());
            }

            recvCache = null;
        }

        public bool DropCache() {
            if (recvCache.Count > 0) {
                recvCache.Clear();
                return true;
            }

            return false;
        }

        public byte[] ReadAll() { // possible performance influence
            if (!Connected) throw new InvalidOperationException(ErrMsg.NotConnected);
            if (recvCache.Count == 0) throw new InvalidOperationException(ErrMsg.NoDataAvailable);
            var rv = recvCache.ToArray();

            recvCache.Clear();
            return rv;
        }

        public string ReadAllAsString(Encoding encoding = null) {
            encoding = encoding ?? DefaultEncoding;

            return encoding.GetString(ReadAll());
        }

        // connect
        public void Connect(IPEndPoint remote) {
            if (Connected) throw new InvalidOperationException(ErrMsg.AlreadyConnected);

            client = new TcpClient();

            try {
                client.Connect(remote);

                new Thread(receiver).Start();
                beginReceive();
            } catch (Exception ex) {
                if (client.Connected) client.Close();

                client = null;
                throw ex;
            }
        }

        public void Disconnect() {
            if (!Connected) throw new InvalidOperationException(ErrMsg.NotConnected);

            endReceive();

            if (client.Connected) client.Close();
            client = null;
        }
    }
}
