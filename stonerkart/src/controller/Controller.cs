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

        public static User user { get; private set; }

        public static void launchGame()
        {
            //if (!Network.connectToServer()) throw new Exception("Server down for more or less routine maintenance.");
            GUI.launch();
            GUI.transitionToScreen(GUI.deckeditorScreen);
        }

        public static void attemptLogin(string username, string password)
        {
            if (Network.login(username, password))
            {
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

                GUI.transitionToScreen(GUI.mainMenuScreen);
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

        public static bool makePurchase(ProductUnion product)
        {
            int newbalance = Network.makePurchase(product);
            if (newbalance == -1) return false; //purchase failed
            setShekelBalance(newbalance);
            return true;
        }

        public static void setShekelBalance(int i)
        {
            GUI.frame.menu.setShekelCount(i);
        }

        public static Game startGame(NewGameStruct ngs)
        {
            GameScreen gsc = new GameScreen();
            Game g = new Game(ngs, false, gsc);
            GUI.transitionToScreen(gsc);
            g.startGameThread();
            return g;
        }

        public static void quit()
        {
            Settings.saveSettings();
            Environment.Exit(1);
        }

        public static void challengePlayer(string username)
        {
            Network.challenge(username);
        }

    }
}
