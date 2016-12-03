using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
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
        private const string servername = "_server";
        private static ManualResetEvent messageReceived = new ManualResetEvent(false);
        private static Message receivedMessage;

        public static bool connectToServer()
        {
            try
            {
                serverConnection = new ServerConnection();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static void handle(Message m)
        {
            receivedMessage = m;
            if (m.messageType == Message.MessageType.RESPONSE)
            {
                receivedMessage = m;
                messageReceived.Set();
            }
        }

        private static Message awaitResponseMessage()
        {
            messageReceived.WaitOne();
            Message r = receivedMessage;
            receivedMessage = null;
            messageReceived.Reset();
            return r;
        }

        public static bool login(string username, string password)
        {
            LoginBody b = new LoginBody(username, password);
            serverConnection.send(new Message(servername, Message.MessageType.LOGIN, b.toBody()));
            Message m = awaitResponseMessage();
            ResponseBody rb = new ResponseBody(m.body);
            return rb.code == ResponseBody.ResponseCode.OK;
        }

        public static bool register(string username, string password)
        {
            LoginBody b = new LoginBody(username, password);
            serverConnection.send(new Message(servername, Message.MessageType.REGISTER, b.toBody()));
            return false;
        }

    }
}
