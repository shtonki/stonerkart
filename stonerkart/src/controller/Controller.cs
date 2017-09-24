using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace stonerkart
{
    /// <summary>
    /// 
    /// </summary>
    static class Controller
    {

        private static List<Packs> ownedPacks { get; set; }
        private static List<CardTemplate> ownedCards { get; set; }

        public static IEnumerable<Packs> OwnedPacks => ownedPacks;
        public static IEnumerable<CardTemplate> OwnedCards => ownedCards;

        public static List<Game> ActiveGames { get; } = new List<Game>();

        public static User user { get; private set; }

        public static void launchGame()
        {
            //if (!Network.connectToServer()) throw new Exception("Server down for more or less routine maintenance.");
            GUI.launch();
            GUI.transitionToScreen(GUI.loginScreen);
        }

        public static void attemptLogin(string username, string password)
        {
            var lrm = Network.login(username, password);
            if (lrm.Success)
            {
                var myUser = lrm.loggedInUserFullJist.ToUser();

                if (user != null) throw new Exception();
                user = myUser;
                GUI.frame.SetUser(user);
                GUI.transitionToScreen(GUI.mainMenuScreen);

                return;

                throw new NotImplementedException("if you weren't expecting too see this you might be in some trouble son");
                /*
                user = Network.queryMyUser();
                GUI.frame.loginAs(user);

                var friends = Network.queryFriends();
                user.setFriends(friends);

                var friendrequests = Network.queryFriendRequests();
                GUI.frame.addFriendsPanel.addRequests(friendrequests);

                var collection = Network.queryCollection();
                ownedCards = collection.ToList();

                var shekels = Network.queryShekels();
                setShekelBalance(shekels);

                var ownedPacks = Network.queryOwnedPacks();
                GUI.shopScreen.populate(ownedPacks);

                */
            }
            else
            {
                
            }
        }

        public static void respondToFriendRequest(string username, bool accept)
        {
            Network.respondToFriendRequest(username, accept);
        }

        public static bool ripPack(Packs pack)
        {
            var ripped = Network.ripPack(pack);
            if (ripped == null) return false;
            else
            {
                GUI.shopScreen.ripPack(ripped);
                ownedCards.AddRange(ripped);
                return true;
            }
        }

        public static bool makePurchase(Product product)
        {
            int newbalance = Network.makePurchase(product);
            if (newbalance == -1) return false; //purchase failed
            user.Shekels = newbalance;
            user.AddOwnedProduct(product);
            return true;
        }


        public static Game StartGame(GameSetupInfo gsi)
        {
            Game g = new Game(gsi);
            GameScreen gsc = new GameScreen(gsi);
            GUI.transitionToScreen(gsc);
            g.coupleToScreen(gsc);
            ActiveGames.Add(g);
            g.start();
            return g;
        }

        public static void quit()
        {
            Settings.saveSettings();
            Environment.Exit(1);
        }

        public static void challengePlayer(string username)
        {
            GameRules rules = new GameRules(MapConfiguration.Default);

            Network.challenge(username, rules);
        }

        public static void GetChallenged(ChallengeMessage cm)
        {
            var option = GUI.promptUser(cm.ChallengeeName + " challenges you to a battle to the death. Do you wish to accept this challenge?",
                ButtonOption.Yes, ButtonOption.No);
            Network.AcceptChallenge(cm, option == ButtonOption.Yes);
        }

        public static void AnswerChallenge(ChallengeResponseMessage acm)
        {
            
        }


    }
}
