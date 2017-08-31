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

        private static List<Game> activeGames = new List<Game>();

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
                case Message.MessageType.QUERYRESPONSE:
                case Message.MessageType.RESPONSE:
                {
                    receivedMessage = m;
                    messageReceived.Set();
                } break;

                case Message.MessageType.CHALLENGE:
                {
                    ChallengeBody cb = new ChallengeBody(m.body);
                    serverConnection.send(new Message("_server", Message.MessageType.ACCEPTCHALLENGE, new ChallengeBody(cb.challengee)));
                } break;

                case Message.MessageType.NEWGAME:
                {
                    NewGameBody b = new NewGameBody(m.body);
                    Game g = Controller.startGame(b.newGameStruct);
                    activeGames.Add(g);
                    
                } break;

                case Message.MessageType.GAMEMESSAGE:
                {
                    GameMessageBody b = new GameMessageBody(m.body);
                    var gm = activeGames.First(g => g.gameid == b.gameid);
                    gm.enqueueGameMessage(b.message);
                } break;

                case Message.MessageType.ENDGAME:
                {
                    EndGameMessageBody b = new EndGameMessageBody(m.body);
                    var gm = activeGames.First(g => g.gameid == b.gameid);
                    gm.endGame(b.ges);
                } break;

                default:
                    throw new Exception(m.messageType.ToString());
            }
        }

        public static void sendGameMessage(int gameid, string message)
        {
            GameMessageBody b = new GameMessageBody(gameid, message);
            Message m = new Message(servername, Message.MessageType.GAMEMESSAGE, b);
            serverConnection.send(m);
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
            if (rb.code == ResponseBody.ResponseCode.OK)
            {
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

        public static string[] queryFriends()
        {
            QueryBody b = new QueryBody(Queries.FRIENDS);
            serverConnection.send(new Message(servername, Message.MessageType.QUERY, b));
            Message m = awaitResponseMessage();
            QueryResponseBody rb = new QueryResponseBody(m.body);
            return rb.values;
        }

        public static bool matchmake()
        {
            MatchmakemeBody mmb = new MatchmakemeBody();
            serverConnection.send(new Message(servername, Message.MessageType.MATCHMAKEME, mmb));
            return true;
        }

        public static void surrender(int gameid, GameEndStateReason reason)
        {
            SurrenderMessageBody egmb = new SurrenderMessageBody(gameid, reason);
            serverConnection.send(new Message(servername, Message.MessageType.SURRENDER, egmb));
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
