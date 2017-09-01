using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace stonerkart
{

    static class Network
    {
        private static ServerConnection serverConnection;
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

                case Message.MessageType.USERSTATUSCHANGED:
                {
                    var body = new UserStatusChangedBody(m.body);
                    var friend = Controller.user.Friends.First(u => u.Name == body.username);
                    friend.Status = body.status;
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

        private static ResponseBody askServer(Message.MessageType messageType, MessageBody body)
        {
            tellServer(messageType, body);
            Message message = awaitResponseMessage();
            if (message.messageType != Message.MessageType.RESPONSE) throw new Exception();
            ResponseBody rb = new ResponseBody(message.body);
            return rb;
        }

        private static QueryResponseBody queryServer(Queries query)
        {
            tellServer(Message.MessageType.QUERY, new QueryBody(query));
            Message message = awaitResponseMessage();
            if (message.messageType != Message.MessageType.QUERYRESPONSE) throw new Exception();
            QueryResponseBody rb = new QueryResponseBody(message.body);
            return rb;
        }

        private static bool tellServer(Message.MessageType messageType, MessageBody body)
        {
            serverConnection.send(new Message(servername, messageType, body));
            return true;
        } 

        public static bool login(string username, string password)
        {
            ResponseBody rb = askServer(
                Message.MessageType.LOGIN,
                new LoginBody(username, password));

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

        public static User queryMyUser()
        {
            var rb = queryServer(Queries.MYCOSMETICS);
            if (rb.values.Length != 1) throw new Exception();
            return User.FromString(rb.values[0]);
        }

        public static User[] queryFriends()
        {
            QueryResponseBody rb = queryServer(Queries.FRIENDS);
            return rb.values.Select(User.FromString).ToArray();
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
