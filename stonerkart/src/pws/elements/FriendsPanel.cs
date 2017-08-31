using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{

    class FriendsPanel : Square
    {
        private List<FriendBar> friends = new List<FriendBar>();
        private const int friendBarHeight = 100;

        public FriendsPanel(int width, int height) : base(width, height)
        {
        }

        public void addFriends(IEnumerable<User> friends)
        {
            foreach (var friend in friends)
            {
                FriendBar fb = new FriendBar(Width, friendBarHeight, friend);
                addChild(fb);
            }

            layoutStuff();
        }

        private void layoutStuff()
        {
            for (int i = 0; i < friends.Count; i++)
            {
                var fb = friends[i];

                if ((i + 1)*friendBarHeight < Height)
                {
                    fb.Y = i*friendBarHeight;
                    fb.Visible = true;
                }
                else
                {
                    fb.Visible = false;
                }
            }
        }
    }

    class FriendBar : Square
    {
        public User friend { get; }
        private Button name;
        private Button challenge;

        public FriendBar(int width, int height, User friend) : base(width, height)
        {
            if (width < height) throw new Exception();

            this.friend = friend;
            challenge = new Button(height, height);
            challenge.X = width - height;
            challenge.Backimege = new Imege(Textures.buttonChallenge);

            name = new Button(width - height, height);
            name.Text = friend.name;
        }
    }
}
