using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class ServerConnection : AsyncConnection
    {
        private const string serverIp =
            //"127.0.0.1";
            "46.239.124.155";

        public ServerConnection() : base(generateSocket())
        {
        }

        protected override void handle(Message m)
        {
            Network.handle(m);
        }

        protected override void closed()
        {
            throw new NotImplementedException();
        }

        private static Socket generateSocket()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IAsyncResult result = socket.BeginConnect(serverIp, 420, null, null);

            bool success = result.AsyncWaitHandle.WaitOne(2500, true);

            if (!success)
            {
                socket.Close();
                throw new ApplicationException("server fucked");
            }

            return socket;
        }
    }
}
