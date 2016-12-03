using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace stonerkart
{
    interface SyncConnection
    {
        Message receive();
        void send(Message m);
    }

    

    static class Network
    {
        public static ServerConnection serverConnection;

        public static bool connectToServer()
        {
            try
            {
                serverConnection = new ServerConnection();
                //return true;
            }
            catch (Exception e)
            {
                return false;
            }

            while (true)
            {
                string s = System.Console.ReadLine();
                serverConnection.send(new Message("_server", Message.MessageType.ECHO, s));
            }
        }
    }
}
