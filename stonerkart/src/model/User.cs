using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace stonerkart
{
    [Serializable]
    public class FullUserJist : BasicUserJist
    {
        public BasicUserJist[] friends { get; set; }
        public int shekels { get; set; }
        public int rating { get; set; }
        public CardTemplate[] cards { get; set; }
        public Product[] products { get; set; }
        public BasicUserJist[] friendRequests { get; set; }


        public FullUserJist()
        {
        }

        public FullUserJist(User user) : base(user)
        {
            friends = user.Friends.Select(u => new BasicUserJist(u)).ToArray();
            friendRequests = user.FriendRequests.Select(u => new BasicUserJist(u)).ToArray();
            shekels = user.Shekels;
            rating = user.Rating;
            cards = user.CardCollection.ToArray();
            products = user.ProductCollection.ToArray();
        }

        public override User ToUser()
        {
            List<User> friendList = friends.Select(f => f.ToUser()).ToList();
            List<User> friendRequestList = friendRequests.Select(f => f.ToUser()).ToList();
            List<CardTemplate> cardList = cards.ToList();
            List<Product> productList = products.ToList();
            return new User(
                name, 
                icon, 
                title, 
                status, 
                friendList, 
                friendRequestList, 
                rating, 
                shekels, 
                cardList,
                productList
                );
        }
    }

    interface UserJist
    {
        User ToUser();
    }

    [Serializable]
    public class BasicUserJist : UserJist
    {
        public string name { get; set; }
        public CardTemplate icon { get; set; }
        public Title title { get; set; }
        public UserStatus status { get; set; }

        public BasicUserJist()
        {
        }

        public BasicUserJist(User user)
        {
            name = user.Name;
            icon = user.Icon;
            title = user.Title;
            status = user.Status;
        }

        public virtual User ToUser()
        {
            return new User(name, icon, title, status, new User[0]);
        }
    }

    public class User : Observable<UserChanged>
    {
        private string name;
        private CardTemplate icon;
        private Title title;

        private UserStatus status;
        private List<User> friends;
        private List<User> friendRequests;

        private int rating;
        private int shekels;
        private List<CardTemplate> cardCollection;
        private List<Product> productCollection;

        private List<ChallengeMessage> pendingChallenges;

        public IEnumerable<User> Friends => friends;
        public IEnumerable<ChallengeMessage> PendingChallenges => pendingChallenges;

        public User(string name, CardTemplate icon, Title title, UserStatus status, IEnumerable<User> friends, IEnumerable<User> friendRequests, int rating, int shekels, IEnumerable<CardTemplate> cardCollection, IEnumerable<Product> productCollection)
        {
            this.name = name;
            this.icon = icon;
            this.title = title;
            this.status = status;
            this.friends = friends.ToList();
            this.friendRequests = friendRequests.ToList();
            this.rating = rating;
            this.shekels = shekels;
            this.cardCollection = cardCollection.ToList();
            this.productCollection = productCollection.ToList();
        }

        public User(string name, CardTemplate icon, Title title, UserStatus status, IEnumerable<User> friends)
        {
            this.name = name;
            this.icon = icon;
            this.title = title;
            this.status = status;
            this.friends = friends.ToList();
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

        public virtual UserStatus Status
        {
            get { return status; }
            set
            {
                status = value;
                notify(new UserChanged(this));
            }
        }

        public int Shekels
        {
            get { return shekels; }
            set
            {
                shekels = value;
                notify(new UserChanged(this));
            }
        }

        public List<CardTemplate> CardCollection
        {
            get { return cardCollection; }
        }

        public List<Product> ProductCollection
        {
            get { return productCollection; }
        }

        public int Rating
        {
            get { return rating; }
        }

        public List<User> FriendRequests
        {
            get { return friendRequests; }
        }


        public void addFriend(User friend)
        {
            friends.Add(friend);
            notify(new UserChanged(this));
        }

        public void AddOwnedProduct(Product product)
        {
            productCollection.Add(product);
            notify(new UserChanged(this));
        }

        public void AddFriendRequest(User requester)
        {
            friendRequests.Add(requester);
            notify(new UserChanged(this));
        }
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