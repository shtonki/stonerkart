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
            User user = (User)o;
            //todo 210917 this leaves dead observers in friends
            var newfriends = user.Friends;
            clearChildren();
            friends.Clear();
            addFriends(newfriends);
        }

        private void addFriends(IEnumerable<User> newFriends)
        {
            foreach (var friend in newFriends)
            {
                FriendBar fb = new FriendBar(Width, friendBarHeight, friend);
                friend.AddObserver(fb);
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
        private PlayerFlarePanel playerflare;
        private Button challenge;

        public FriendBar(int width, int height, User friend) : base(width, height)
        {
            if (width < height) throw new Exception();


            Backcolor = backcolorFromStatus(friend.Status);

            challenge = new Button(height, height);
            addChild(challenge);
            challenge.Backcolor = Color.Silver;
            challenge.X = width - height;
            challenge.Backimege = new Imege(Textures.iconChallenge);
            challenge.Visible = friend.Status != UserStatus.Offline;
            challenge.clicked += a => Controller.challengePlayer(friend.Name);

            playerflare = new PlayerFlarePanel(width - height, height);
            friend.AddObserver(playerflare);
            addChild(playerflare);

            friend.AddObserver(this);
        }

        public void notify(object o, UserChanged t)
        {
            User user = (User)o;
            Backcolor = backcolorFromStatus(user.Status);
            challenge.Visible = user.Status != UserStatus.Offline;
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

    class PendingFriendsPanel : Square, Observer<UserChanged>
    {
        private const int okbtnsize = 40;
        private const int requestHeight = 50;
        private const int requestPadding = 20;

        private Square requestsSquare;

        private List<PenderPanel> pendingRequests { get; } = new List<PenderPanel>();

        public PendingFriendsPanel(int width, int height) : base(width, height)
        {
            InputBox friendname = new InputBox(width - okbtnsize, okbtnsize);
            addChild(friendname);

            Button okbtn = new Button(okbtnsize, okbtnsize);
            addChild(okbtn);
            okbtn.X = width - okbtnsize;
            okbtn.Backimege = new Imege(Textures.iconCheck);
            okbtn.clicked += a => { if (Network.sendFriendRequest(friendname.Text)) friendname.Text = ""; };

            requestsSquare = new Square(width, height - okbtnsize*2);
            addChild(requestsSquare);
            requestsSquare.Y = okbtnsize*2;
        }

        private void layoutStuff()
        {
            requestsSquare.clearChildren();
            int maxmemes = 100;
            for (int i = 0; i < pendingRequests.Count; i++)
            {
                var pr = pendingRequests[i];
                requestsSquare.addChild(pr);
                pr.Y = requestPadding + i*(requestPadding + requestHeight);
            }
        }

        public void addRequests(IEnumerable<string> requests)
        {
            foreach (var requester in requests)
            {
                string rqster = requester;

                var pp = new PenderPanel(Width, requestHeight, requester);
                pendingRequests.Add(pp);

                pp.decline.clicked += a =>
                {
                    koenmeme(rqster, false);
                };
                pp.accept.clicked += a =>
                {
                    koenmeme(rqster, true);
                };
            }
            layoutStuff();
        }

        private void koenmeme(string username, bool accept)
        {
            Controller.respondToFriendRequest(username, accept);
            pendingRequests.RemoveAll(pnl => pnl.Name == username);
            layoutStuff();
        }

        public void notify(object o, UserChanged t)
        {
            User usr = (User)o;
            pendingRequests.Clear();
            addRequests(usr.FriendRequests.Select(u => u.Name));
        }
    }

    class PenderPanel : Square
    {
        public readonly string Name;
        public Button accept { get; }
        public Button decline { get; }

        public PenderPanel(int width, int height, string name) : base(width, height)
        {
            Name = name;
            Backcolor = Color.FromArgb(150, 20, 20, 200);

            Square namepanel = new Square(width-height, height);
            addChild(namepanel);
            namepanel.Text = name;

            accept = new Button(height/2, height/2);
            addChild(accept);
            accept.Backimege = new Imege(Textures.iconCheck);
            accept.X = width - height / 4 * 6;
            accept.Y = height / 4;

            decline = new Button(height/2, height/2);
            addChild(decline);
            decline.Backimege = new Imege(Textures.iconCross);
            decline.X = width - height/4 * 3;
            decline.Y = height/4;
        }
    }
}
