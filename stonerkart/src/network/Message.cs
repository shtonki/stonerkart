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

        public Message(string recipient, MessageType messageType, string body)
        {
            this.recipient = recipient;
            this.messageType = messageType;
            this.body = body;
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
        }
    }

    class LoginBody
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

    class ResponseBody
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
        }
    }
}