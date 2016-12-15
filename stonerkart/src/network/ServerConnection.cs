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
            EndPoint endpoint = new IPEndPoint(IPAddress.Parse("82.196.98.15"), 420);
            Socket socket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(endpoint);
            return socket;
        }
    }
}
