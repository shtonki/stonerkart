using System;
using System.Text;

namespace stonerkart
{
    public class User
    {
        public string name { get; }
        public CardTemplate icon { get; }
        public Title title { get; }

        public User(string name, CardTemplate icon, Title title)
        {
            this.name = name;
            this.icon = icon;
            this.title = title;
        }

        public static User FromString(string s)
        {
            var ss = s.Split(separator);

            var username = ss[0];

            CardTemplate usericon;
            if (!CardTemplate.TryParse(ss[1], out usericon)) throw new Exception();

            Title usertitle;
            if (!Title.TryParse(ss[2], out usertitle)) throw new Exception();

            return new User(username, usericon, usertitle);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(name);
            sb.Append(separator);

            sb.Append(icon);
            sb.Append(separator);

            sb.Append(title);
            sb.Append(separator);

            return sb.ToString();
        }

        private const char separator = ':';
    }

    public enum Title
    {
        NOTITLE,

        Memelord,

        POWERBREAK,

        of_sThe_sMeme,
    }
}