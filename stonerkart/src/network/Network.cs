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

        //todo change this
        private static ManualResetEvent messageReceived = new ManualResetEvent(false);
        private static Message receivedMessage;

        public static bool Connected => connected;

        private static bool connected;


        public static void TryConnect()
        {
            if (Connected) return;
            if (ConnectToServer()) connected = true;
        }

        private static bool ConnectToServer()
        {
            if (serverConnection != null) return true;

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
            if (m.demandsResponse)
            {
                receivedMessage = m;
                messageReceived.Set();
                return;
            }
            else if (m is GameMessage)
            {
                var msg = (GameMessage)m;
                Game game = Controller.ActiveGames.Find(g => g.gameid == msg.gameid);
                game.enqueueGameMessage(msg);
            }
            else if (m is UserStatusChangedMessage)
            {
                UserStatusChangedMessage msg = (UserStatusChangedMessage)m;
                Controller.user.Friends.First(f => f.Name == msg.username).Status = msg.newStatus;
            }
            else if (m is ChallengeMessage)
            {
                ChallengeMessage msg = (ChallengeMessage)m;
                Controller.GetChallenged(msg);
            }
            else if (m is StartGameMessage)
            {
                StartGameMessage msg = (StartGameMessage)m;
                Controller.StartGame(msg.GameSetupInfo);
            }
            else throw new NotImplementedException("if you weren't expecting too see this you might be in some trouble son");

            /*

                case Message.MessageType.NEWGAME:
                {
                    NewGameBody b = new NewGameBody(m.body);
                    Game g = Controller.startGame(b.newGameStruct);
                } break;

                case Message.MessageType.GAMEMESSAGE:
                {
                    GameMessageBody b = new GameMessageBody(m.body);
                    var gm = Controller.GetActiveGame(b.gameid);
                    gm.enqueueGameMessage(b.message);
                } break;

                case Message.MessageType.ENDGAME:
                {
                    EndGameMessageBody b = new EndGameMessageBody(m.body);
                    var gm = Controller.GetActiveGame(b.gameid);
                    gm.endGame(b.ges);
                } break;

                case Message.MessageType.USERSTATUSCHANGED:
                {
                    var body = new UserStatusChangedBody(m.body);
                    var friend = Controller.user.Friends.First(u => u.Name == body.username);
                    friend.Status = body.status;
                } break;

                case Message.MessageType.FRIENDREQUEST:
                {
                    AddFriendBody body = new AddFriendBody(m.body);
                    GUI.frame.addFriendsPanel.addRequests(new [] {body.name});
                } break;

                case Message.MessageType.ACCEPTFRIEND:
                {
                    var body = new AddFriendBody(m.body);
                    var newfriend = User.FromString(body.name);
                    GUI.frame.friendsPanel.addFriends(new [] {newfriend});
                    Controller.user.addFriend(newfriend);
                } break;

                default:
                    throw new Exception(m.messageType.ToString());
            } */
        }

        public static UserQueryResponseMessage userQuery(string username)
        {

            UserQueryMessage qm = new UserQueryMessage();
            qm.username = username;
            return (UserQueryResponseMessage)askServer(qm);
        }

        public static void sendGameMessage(int gameid, string message)
        {
            GameMessage m = new GameMessage();
            m.gameid = gameid;
            m.message = message;
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

        static Random msgresponseidgenerator = new Random();

        private static Message askServer(Message msg)
        {
            var rid = msgresponseidgenerator.Next();
            msg.responseID = rid;
            tellServer(msg);
            Message response = awaitResponseMessage();
            if (response.responseID != rid) throw new Exception();
            return response;
        }

        /*
        private static QueryResponseBody queryServer(Queries query)
        {
            tellServer(Message.MessageType.QUERY, new QueryBody(query));
            Message message = awaitResponseMessage();
            if (message.messageType != Message.MessageType.QUERYRESPONSE) throw new Exception();
            QueryResponseBody rb = new QueryResponseBody(message.body);
            return rb;
        }
        */
        private static bool tellServer(Message msg)
        {
            serverConnection.send(msg);
            return true;
        } 

        public static LoginResponseMessage login(string username, string password)
        { 
            var loginMessage = new LoginMessage();
            loginMessage.username = username;
            loginMessage.password = password;

            return (LoginResponseMessage)askServer(loginMessage);
        }

        public static bool sendFriendRequest(string username)
        {
            var addFriendMessage = new FriendRequestMessage();
            addFriendMessage.username = username;
            var response = (ResponseMessage)askServer(addFriendMessage);
            return response.code == ResponseMessage.Code.OK;
        }

        public static void respondToFriendRequest(string requester, bool accept)
        {
            var respondToFriendRequestMessage = new RespondToFriendRequestMessage();
            respondToFriendRequestMessage.requester = requester;
            respondToFriendRequestMessage.accept = accept;
            tellServer(respondToFriendRequestMessage);
        }

        public static bool challenge(string challengee, GameRules gameRules)
        {
            ChallengeMessage b = new ChallengeMessage();
            b.ChallengeeName = challengee;
            b.GameRules = gameRules;
            ResponseMessage rm = (ResponseMessage)askServer(b);
            return rm.code == ResponseMessage.Code.OK;
        }

        public static void AcceptChallenge(ChallengeMessage cm, bool accept)
        {
            var acm = new ChallengeResponseMessage();
            acm.Challenge = cm;
            acm.Accept = accept;
            tellServer(acm);
        }

        /*public static User queryMyUser()
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

        public static IEnumerable<string> queryFriendRequests()
        {
            var rb = queryServer(Queries.FRIENDREQUESTS);
            return rb.values;
        }

        public static IEnumerable<CardTemplate> queryCollection()
        {
            var rb = queryServer(Queries.COLLECTION);
            return rb.values.Select(i => (CardTemplate)Int32.Parse(i));
        }

        public static int queryShekels()
        {
            var rb = queryServer(Queries.SHEKELS);
            if (rb.values.Length != 1) throw new Exception();
            return Int32.Parse(rb.values[0]);
        }

        public static Packs[] queryOwnedPacks()
        {
            var rv = queryServer(Queries.OWNEDPACKS);
            return rv.values.Select(i => (Packs)Int32.Parse(i)).ToArray();
        }
        */
        public static int makePurchase(Product product)
        {
            PurchaseMessage pm = new PurchaseMessage();
            pm.product = product;
            var sr = askServer(pm);
            var response = (PurchaseResponseMessage)sr;
            return response.newBalance;
        }

        public static IEnumerable<CardTemplate> ripPack(Packs pack)
        {

            throw new NotImplementedException("if you weren't expecting too see this you might be in some trouble son");
            /*
            var v = askServer(Message.MessageType.RIPPACK, new ProductBody(new ProductUnion(pack)));
            if (v.code == ResponseBody.ResponseCode.FAILEDGENERIC) return null;
            else
            {
                var body = v.text;
                var ss = body.Split(':');
                return ss.Select(s => (CardTemplate)Int32.Parse(s));
            }
            */
        }


        public static void surrender(int gameid, GameEndStateReason reason)
        {
            throw new NotImplementedException("if you weren't expecting too see this you might be in some trouble son");
            /*
            SurrenderMessageBody egmb = new SurrenderMessageBody(gameid, reason);
            serverConnection.send(new Message(servername, Message.MessageType.SURRENDER, egmb));
            */
        }
    }
    public enum GameEndStateReason { Flop, Surrender, }

}
