using System;
using System.Collections.Generic;
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
        public static ServerConnection serverConnection = null;
        private const string servername = "_server";
        private static ManualResetEvent messageReceived = new ManualResetEvent(false);
        private static Message receivedMessage;

        private static Object GameMessageLockObject = new Object();
        private static Queue<string> gameActionQueue = new Queue<string>();
        private static ManualResetEvent gameMessageReceived = new ManualResetEvent(false);

        public static bool connectToServer()
        {
            if (serverConnection != null) return true;

            try
            {
                serverConnection = new ServerConnection();
                return true;
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
                return false;
            }
        }

        public static void handle(Message m)
        {
            switch (m.messageType)
            {
                case Message.MessageType.RESPONSE:
                {
                    receivedMessage = m;
                    messageReceived.Set();
                } break;

                case Message.MessageType.CHALLENGE:
                {
                    ChallengeBody cb = new ChallengeBody(m.body);
                    serverConnection.send(new Message("_server", Message.MessageType.ACCEPTCHALLENGE, new ChallengeBody(cb.username)));
                } break;

                case Message.MessageType.NEWGAME:
                {
                    NewGameBody b = new NewGameBody(m.body);
                    ScreenController.transitionToGamePanel(b.newGameStruct, false);
                } break;

                case Message.MessageType.GAMEMESSAGE:
                {
                    GameMessageBody b = new GameMessageBody(m.body);
                    enqueueGameMessage(b.message);
                } break;

                default:
                    throw new Exception(m.messageType.ToString());
            }
        }

        private static void enqueueGameMessage(string message)
        {
            lock (GameMessageLockObject)
            {
                gameActionQueue.Enqueue(message);
                gameMessageReceived.Set();
            }
        }

        public static string dequeueGameMessage()
        {
            if (gameActionQueue.Count == 0)
            {
                gameMessageReceived.WaitOne();
            }
            lock (GameMessageLockObject)
            {
                string r = gameActionQueue.Dequeue();
                gameMessageReceived.Reset();
                return r;
            }
        }

        public static void sendGameMessage(string message, string[] recipients)
        {
            foreach (string recipient in recipients)
            {
                GameMessageBody b = new GameMessageBody(message);
                Message m = new Message(recipient, Message.MessageType.GAMEMESSAGE, b);
                serverConnection.send(m);
                Thread.Sleep(2);
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
            serverConnection.send(new Message(servername, Message.MessageType.LOGIN, b));
            Message m = awaitResponseMessage();
            ResponseBody rb = new ResponseBody(m.body);
            if (rb.code == ResponseBody.ResponseCode.OKWITHFRIENDS)
            {
                FriendListBody fb = new FriendListBody(rb.text);
                UIController.setFriendList(fb.friends);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool register(string username, string password)
        {
            LoginBody b = new LoginBody(username, password);
            serverConnection.send(new Message(servername, Message.MessageType.REGISTER, b));
            return false;
        }

        public static bool addFriend(string username)
        {
            AddFriendBody b = new AddFriendBody(username);
            serverConnection.send(new Message(servername, Message.MessageType.ADDFRIEND, b));
            Message m = awaitResponseMessage();
            ResponseBody rb = new ResponseBody(m.body);
            return rb.code == ResponseBody.ResponseCode.OK;
        }

        public static bool challenge(string username)
        {
            ChallengeBody b = new ChallengeBody(username);
            serverConnection.send(new Message(servername, Message.MessageType.CHALLENGE, b));
            Message m = awaitResponseMessage();
            ResponseBody rb = new ResponseBody(m.body);
            return rb.code == ResponseBody.ResponseCode.OK;
        }

        public static bool submitBug(string s)
        {
            BugReportBody b = new BugReportBody(s);
            serverConnection.send(new Message(servername, Message.MessageType.BUGREPORT, b));
            Message m = awaitResponseMessage();
            ResponseBody rb = new ResponseBody(m.body);
            return rb.code == ResponseBody.ResponseCode.OK;
        }
    }
}
