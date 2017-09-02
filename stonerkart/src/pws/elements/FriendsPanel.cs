using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{

    class FriendsPanel : Square, Observer<UserChanged>
    {
        private List<FriendBar> friends = new List<FriendBar>();
        private const int friendBarHeight = 65;
        private const int friendBarPadding = 10;

        public FriendsPanel(int width, int height) : base(width, height)
        {
        }

        public void notify(object o, UserChanged t)
        {
            var newfriends = t.user.Friends;
            clearChildren();
            friends.Clear();
            addFriends(newfriends);
        }

        public void addFriends(IEnumerable<User> newFriends)
        {
            foreach (var friend in newFriends)
            {
                FriendBar fb = new FriendBar(Width, friendBarHeight, friend);
                friends.Add(fb);
                addChild(fb);
            }

            layoutStuff();
        }

        private void layoutStuff()
        {
            for (int i = 0; i < friends.Count; i++)
            {
                var fb = friends[i];

                if ((i + 1)*(friendBarHeight + friendBarPadding) < Height)
                {
                    fb.Y = i* (friendBarHeight + friendBarPadding);
                    fb.Visible = true;
                }
                else
                {
                    fb.Visible = false;
                }
            }
        }
    }

    class FriendBar : Square, Observer<UserChanged>
    {
        public User friend { get; }
        private PlayerFlarePanel playerflare;
        private Button challenge;

        public FriendBar(int width, int height, User friend) : base(width, height)
        {
            if (width < height) throw new Exception();

            this.friend = friend;
            friend.addObserver(this);

            Backcolor = backcolorFromStatus(friend.Status);

            challenge = new Button(height, height);
            addChild(challenge);
            challenge.Backcolor = Color.Silver;
            challenge.X = width - height;
            challenge.Backimege = new Imege(Textures.iconChallenge);

            playerflare = new PlayerFlarePanel(friend.Name, friend.Icon, width - height, height);
            addChild(playerflare);
        }

        public void notify(object o, UserChanged t)
        {
            Backcolor = backcolorFromStatus(friend.Status);
        }

        private static Color backcolorFromStatus(UserStatus status)
        {
            switch (status)
            {
                case UserStatus.Offline: return Color.Silver;
                case UserStatus.Online: return Color.DarkGreen;
            }
            throw new Exception();
        }
    }
}
