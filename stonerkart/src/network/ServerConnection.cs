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

            IAsyncResult result = socket.BeginConnect("82.196.98.15", 420, null, null);

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
