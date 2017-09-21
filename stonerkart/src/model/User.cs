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

        public FullUserJist()
        {
        }

        public FullUserJist(string name, CardTemplate icon, Title title, UserStatus status, BasicUserJist[] friends, int shekels, IEnumerable<CardTemplate> cards, IEnumerable<Product> products) : base(name, icon, title, status)
        {
            this.friends = friends;
            this.shekels = shekels;
            this.cards = cards.ToArray();
            this.products = products.ToArray();
        }

        public override User ToUser()
        {
            List<User> fs = friends.Select(f => f.ToUser()).ToList();
            List<CardTemplate> cs = cards.ToList();
            List<Product> ps = products.ToList();
            return new User(name, icon, title, status, fs, rating, shekels, cs, ps);
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

        public BasicUserJist(string name, CardTemplate icon, Title title, UserStatus status)
        {
            this.name = name;
            this.icon = icon;
            this.title = title;
            this.status = status;
        }

        public virtual User ToUser()
        {
            return new User(name, icon, title, status, null);
        }
    }

    public class User : Observable<UserChanged>
    {
        private string name;
        private CardTemplate icon;
        private Title title;

        private UserStatus status;
        private List<User> friends;

        private int rating;
        private int shekels;
        private List<CardTemplate> cardCollection;
        private List<Product> productCollection;



        public IEnumerable<User> Friends => friends;

        public User(string name, CardTemplate icon, Title title, UserStatus status, List<User> friends, int rating, int shekels, List<CardTemplate> cardCollection, List<Product> productCollection)
        {
            this.name = name;
            this.icon = icon;
            this.title = title;
            this.status = status;
            this.friends = friends;
            this.rating = rating;
            this.shekels = shekels;
            this.cardCollection = cardCollection;
            this.productCollection = productCollection;
        }

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

        public void addFriend(User friend)
        {
            friends.Add(friend);
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