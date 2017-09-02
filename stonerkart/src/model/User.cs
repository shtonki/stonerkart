using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace stonerkart
{
    public class User : Observable<UserChanged>
    {
        private string name;
        private CardTemplate icon;
        private Title title;
        private UserStatus status;
        private List<User> friends;

        public IEnumerable<User> Friends => friends;

        public User(string name, CardTemplate icon, Title title, UserStatus status, List<User> friends)
        {
            this.name = name;
            this.icon = icon;
            this.title = title;
            this.status = status;
            this.friends = friends;
        }

        public string Name
        {
            get { return name; }
        }

        public CardTemplate Icon
        {
            get { return icon; }
        }

        public Title Title
        {
            get { return title; }
        }

        public UserStatus Status
        {
            get { return status; }
            set
            {
                status = value;
                notify(new UserChanged(this));
            }
        }

        public void setFriends(IEnumerable<User> friends)
        {
            if (this.friends != null) throw new Exception();
            this.friends = friends.ToList();
            notify(new UserChanged(this));
        }

        public static User FromString(string s)
        {
            var ss = s.Split(separator);

            var username = ss[0];

            CardTemplate usericon;
            if (!CardTemplate.TryParse(ss[1], out usericon)) throw new Exception();

            Title usertitle;
            if (!Title.TryParse(ss[2], out usertitle)) throw new Exception();

            UserStatus userStatus;
            if (!UserStatus.TryParse(ss[3], out userStatus)) throw new Exception();

            return new User(username, usericon, usertitle, userStatus, null);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(Name);
            sb.Append(separator);

            sb.Append(Icon);
            sb.Append(separator);

            sb.Append(Title);
            sb.Append(separator);

            sb.Append(Status);

            return sb.ToString();
        }

        private const char separator = ';';
    }

    public struct UserChanged
    {
        public readonly User user;

        public UserChanged(User user)
        {
            this.user = user;
        }
    }

    public enum UserStatus
    {
        Online,
        Offline,
    }

    public enum Title
    {
        NOTITLE,

        Memelord,

        POWERBREAK,

        of_sThe_sMeme,
    }
}