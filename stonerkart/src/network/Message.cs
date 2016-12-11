using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace stonerkart
{
    class Message
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
            ECHO,
            LOGIN,
            REGISTER,
            RESPONSE,
            ADDFRIEND,
            CHALLENGE,
            ACCEPTCHALLENGE,
            NEWGAME,
            GAMEMESSAGE,
        }
    }

    interface MessageBody
    {
        string toBody();
    }

    class LoginBody : MessageBody
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

    class ResponseBody : MessageBody
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
            OKWITHFRIENDS,
            FAILEDGENERIC,
            FAILEDREQUIRESLOGIN,
        }
    }

    class AddFriendBody : MessageBody
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

    class FriendListBody : MessageBody
    {
        public List<string> friends;

        public FriendListBody(List<string> fs)
        {
            friends = fs;
        }

        public FriendListBody(string s)
        {
            friends = s.Split(':').ToList();
        }

        public string toBody()
        {
            if (friends.Count == 0) return "";
            StringBuilder b = new StringBuilder();
            b.Append(friends[0]);
            for (int i = 1; i < friends.Count; i++)
            {
                b.Append(':');
                b.Append(friends[i]);
            }
            return b.ToString();
        }
    }

    class GameMessageBody : MessageBody
    {
        public string message;

        public GameMessageBody(string s)
        {
            message = s;
        }

        public string toBody()
        {
            return message;
        }
    }

    class ChallengeBody : MessageBody
    {
        public string username;

        public ChallengeBody(string s)
        {
            this.username = s;
        }

        public string toBody()
        {
            return username;
        }
    }

    class NewGameStruct
    {
        public readonly int randomSeed;
        public readonly string[] playerNames;
        public readonly int heroIndex;

        public NewGameStruct(int randomSeed, string[] playerNames, int heroIndex)
        {
            this.randomSeed = randomSeed;
            this.playerNames = playerNames;
            this.heroIndex = heroIndex;
        }
    }

    class NewGameBody : MessageBody
    {
        public NewGameStruct newGameStruct;

        public NewGameBody(int randoSeed, string[] playerNames, int heroIndex)
        {
            newGameStruct = new NewGameStruct(randoSeed, playerNames, heroIndex);
        }

        public NewGameBody(string body)
        {
            string[] ss = body.Split(',');
            int randoSeed = Int32.Parse(ss[0]);
            string[] names = ss[1].Split(':');
            string [] playerNames = names.Where(n => n.Length > 0).ToArray();
            int heroIndex = Int32.Parse(ss[2]);
            newGameStruct = new NewGameStruct(randoSeed, playerNames, heroIndex);
        }

        public string toBody()
        {
            StringBuilder sb = new StringBuilder();
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