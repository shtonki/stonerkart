using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace stonerkart
{
    public class Message
    {
        public string recipient;
        public MessageType messageType;
        public string body;

        private static char divider = (char)0;

        public Message(string s)
        {
            int[] fks = s.Select((c, i) => c == divider ? i : -1).Where(v => v >= 0).ToArray();

            recipient = s.Substring(0, fks[0]);
            string mttext = s.Substring(fks[0]+1, fks[1]-fks[0]-1);
            MessageType.TryParse(mttext, out messageType);
            body = s.Substring(fks[1] + 1, s.Length - fks[1] - 1);
        }

        public Message(string recipient, MessageType messageType, MessageBody b)
        {
            this.recipient = recipient;
            this.messageType = messageType;
            this.body = b.toBody();
        }

        public byte[] getBytes()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(recipient);
            sb.Append(divider);

            sb.Append(messageType.ToString());
            sb.Append(divider);

            sb.Append(body);

            string s = sb.ToString();
            return Encoding.ASCII.GetBytes(s);
        }

        public enum MessageType
        {
            LOGIN,
            REGISTER,
            RESPONSE,
            QUERY,
            QUERYRESPONSE,
            USERSTATUSCHANGED,
            FRIENDREQUEST,
            ACCEPTFRIEND,
            DECLINEFRIEND,
            CHALLENGE,
            ACCEPTCHALLENGE,
            NEWGAME,
            ENDGAME,
            GAMEMESSAGE,
            BUGREPORT,
            MATCHMAKEME,
            SURRENDER,
            MAKEPURCHASE,
            RIPPACK,
        }
    }

    public interface MessageBody
    {
        string toBody();
    }

    public class QueryBody : MessageBody
    {
        public readonly Queries query;

        public QueryBody(string s)
        {
            if (!Enum.TryParse(s, out query)) throw new Exception();
        }

        public QueryBody(Queries query)
        {
            this.query = query;
        }

        public string toBody()
        {
            return query.ToString();
        }
    }

    public enum Queries
    {
        FRIENDS,
        FRIENDREQUESTS,
        MYCOSMETICS,
        SHEKELS,
        COLLECTION,
        OWNEDPACKS,
    };

    public class QueryResponseBody : MessageBody
    {
        private Queries query;
        public readonly string[] values;

        protected const char separator = ':';

        public QueryResponseBody(string s)
        {
            var ss = s.Split(separator);
            if (!Enum.TryParse(ss[0], out query)) throw new Exception();

            values = new string[ss.Length - 1];
            for (int i = 1; i < ss.Length; i++)
            {
                values[i - 1] = ss[i];
            }
        }

        public QueryResponseBody(Queries query, string[] values)
        {
            this.query = query;
            this.values = values;
        }

        public string toBody()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(query.ToString());
            if (values == null || values.Length == 0) return sb.ToString();
            sb.Append(separator);
            foreach (var v in values)
            {
                sb.Append(v);
                sb.Append(separator);
            }
            sb.Length--;
            return sb.ToString();
        }
    }

    public class LoginBody : MessageBody
    {
        public string username;
        public string password;

        public LoginBody(string username, string password)
        {
            this.username = username;
            this.password = password;
        }

        public LoginBody(string s)
        {
            var ss = s.Split(',');
            username = ss[0];
            password = ss[1];
        }

        public string toBody()
        {
            return username + ',' + password;
        }
    }

    public class ResponseBody : MessageBody
    {
        public ResponseCode code;
        public string text;

        public ResponseBody(ResponseCode code, string text)
        {
            this.code = code;
            this.text = text;
        }

        public ResponseBody(string s)
        {
            var ss = s.Split('\0');
            ResponseCode.TryParse(ss[0], out code);
            text = ss[1];
        }

        public string toBody()
        {
            return code.ToString() + '\0' + text;
        }

        public enum ResponseCode
        {
            OK,
            FAILEDGENERIC,
            FAILEDREQUIRESLOGIN,
        }
    }

    public class MatchmakemeBody : MessageBody
    {
        public MatchmakemeBody()
        {

        }

        public MatchmakemeBody(string s)
        {
            
        }

        public string toBody()
        {
            return "";
        }
    }

    public class AddFriendBody : MessageBody
    {
        public string name;

        public AddFriendBody(string name)
        {
            this.name = name;
        }

        public string toBody()
        {
            return name;
        }
    }

    public class UserStatusChangedBody : MessageBody
    {
        public readonly string username;
        public readonly UserStatus status;

        private const char separator = ':';

        public UserStatusChangedBody(string username, UserStatus status)
        {
            this.username = username;
            this.status = status;
        }

        public UserStatusChangedBody(string s)
        {
            var ss = s.Split(separator);
            username = ss[0];
            if (!UserStatus.TryParse(ss[1], out status)) throw new Exception();
        }

        public string toBody()
        {
            return username + separator + status.ToString();
        }
    }

    public class ProductBody : MessageBody
    {
        public ProductUnion product { get; private set; }

        public ProductBody(ProductUnion product)
        {
            this.product = product;
        }

        public ProductBody(string s)
        {
            product = ProductUnion.FromString(s);
        }

        public string toBody()
        {
            return product.ToString();
        }
    }

    public class BugReportBody : MessageBody
    {
        public string text;

        public BugReportBody(string text)
        {
            this.text = text;
        }

        public string toBody()
        {
            return text;
        }
    }
    
    public class GameMessageBody : MessageBody
    {
        public int gameid { get; }
        public string message { get; }

        public GameMessageBody(string s)
        {
            int ix = s.IndexOf('.');
            gameid = Int32.Parse(s.Substring(0, ix));
            message = s.Substring(ix + 1, s.Length - ix - 1);
        }

        public GameMessageBody(int gameid, string s)
        {
            message = s;
            this.gameid = gameid;
        }

        public string toBody()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(gameid);
            sb.Append('.');
            sb.Append(message);

            return sb.ToString();
        }
    }

    public class EndGameMessageBody : MessageBody
    {
        public int gameid { get; }
        public GameEndStruct ges { get; }

        public EndGameMessageBody(int gameid, GameEndStruct ges)
        {
            this.gameid = gameid;
            this.ges = ges;
        }

        public EndGameMessageBody(string s)
        {
            string[] ss = s.Split(';');

            gameid = Int32.Parse(ss[0]);

            string winner = ss[1];

            GameEndStateReason rsn;
            if (!GameEndStateReason.TryParse(ss[2], out rsn)) throw new Exception();

            if (ss[3] == "x")
            {
                ges = new GameEndStruct(winner, rsn);
            }
            else
            {
                int rc = Int32.Parse(ss[3]);
                int yr = Int32.Parse(ss[4]);
                int tr = Int32.Parse(ss[5]);

                ges = new GameEndStruct(winner, rsn, rc, yr, tr);
            }
        }

        public string toBody()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(gameid);
            sb.Append(";");

            sb.Append(ges.winningPlayer);
            sb.Append(";");

            sb.Append(ges.reason);
            sb.Append(";");

            if (ges.yourRating > 0)
            {
                sb.Append(ges.ratingChange);
                sb.Append(";");
                sb.Append(ges.yourRating);
                sb.Append(";");
                sb.Append(ges.theirRating);
            }
            else
            {
                sb.Append("x");
            }

            return sb.ToString();
        }
    }

    public enum GameEndStateReason
    {
        Surrender,
        Flop,
        Decking,
        Ragequit,
    }

    public struct GameEndStruct
    {
        public string winningPlayer { get; }
        public GameEndStateReason reason { get; }
        public int ratingChange { get; }
        public int yourRating { get; }
        public int theirRating { get; }

        public GameEndStruct(string winningPlayer, GameEndStateReason reason, int ratingChange, int yourRating, int theirRating)
        {
            this.winningPlayer = winningPlayer;
            this.reason = reason;
            this.ratingChange = ratingChange;
            this.yourRating = yourRating;
            this.theirRating = theirRating;
        }

        public GameEndStruct(string winningPlayer, GameEndStateReason reason) : this()
        {
            this.winningPlayer = winningPlayer;
            this.reason = reason;
        }
    }

    public class SurrenderMessageBody : MessageBody
    {
        public int gameid { get; }
        public GameEndStateReason reason { get; }

        public SurrenderMessageBody(string s)
        {
            int ix = s.IndexOf('.');
            gameid = Int32.Parse(s.Substring(0, ix));

            GameEndStateReason rsnhck;
            if (!GameEndStateReason.TryParse(s.Substring(ix + 1, s.Length - ix - 1), out rsnhck)) throw new Exception();
            reason = rsnhck;
        }

        public SurrenderMessageBody(int gameid, GameEndStateReason reason)
        {
            this.gameid = gameid;
            this.reason = reason;
        }

        public string toBody()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(gameid);
            sb.Append('.');
            sb.Append(reason);

            return sb.ToString();
        }
    }

    public class ChallengeBody : MessageBody
    {
        public string challengee;

        public ChallengeBody(string s)
        {
            this.challengee = s;
        }

        public string toBody()
        {
            return challengee;
        }
    }

    public class NewGameStruct
    {
        public readonly int gameid;
        public readonly int randomSeed;
        public readonly string[] playerNames;
        public readonly int heroIndex;

        public NewGameStruct(int gameid, int randomSeed, string[] playerNames, int heroIndex)
        {
            this.randomSeed = randomSeed;
            this.playerNames = playerNames;
            this.heroIndex = heroIndex;
            this.gameid = gameid;
        }
    }

    public class NewGameBody : MessageBody
    {
        public NewGameStruct newGameStruct;

        public NewGameBody(int gameid, int randoSeed, string[] playerNames, int heroIndex)
        {
            newGameStruct = new NewGameStruct(gameid, randoSeed, playerNames, heroIndex);
        }

        public NewGameBody(string body)
        {
            string[] ss = body.Split(',');
            int gid = Int32.Parse(ss[0]);
            int randoSeed = Int32.Parse(ss[1]);
            string[] names = ss[2].Split(':');
            string [] playerNames = names.Where(n => n.Length > 0).ToArray();
            int heroIndex = Int32.Parse(ss[3]);
            newGameStruct = new NewGameStruct(gid, randoSeed, playerNames, heroIndex);
        }

        public string toBody()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(newGameStruct.gameid.ToString());
            sb.Append(',');

            sb.Append(newGameStruct.randomSeed.ToString());
            sb.Append(',');

            foreach (string name in newGameStruct.playerNames)
            {
                sb.Append(name);
                sb.Append(':');
            }

            sb.Append(',');
            sb.Append(newGameStruct.heroIndex.ToString());

            return sb.ToString();
        }
    }
}