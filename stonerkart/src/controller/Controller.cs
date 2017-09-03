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
        private static LoginScreen loginScreen = new LoginScreen();
        private static MainMenuScreen mainMenuScreen = new MainMenuScreen();
        private static DeckEditorScreen deckeditorScreen = new DeckEditorScreen();
        private static ShopScreen shopScreen = new ShopScreen();

        private static List<Packs> ownedPacks { get; set; }
        private static List<CardTemplate> ownedCards { get; set; }

        public static IEnumerable<Packs> OwnedPacks => ownedPacks;
        public static IEnumerable<CardTemplate> OwnedCards => ownedCards;

        public static User user { get; private set; }

        public static void launchGame()
        {
            /*
            GUI.launch();
            var gsc = new GameScreen();
            GUI.setScreen(gsc);
            Game g = new Game(new NewGameStruct(0, 420, new[] { "Hero", "Villain" }, 0), true, gsc);
            g.startGameThread();
            return;
            //*/
            GUI.launch();
            GUI.setScreen(new DeckEditorScreen());
            if (!Network.connectToServer()) throw new Exception("Serber offline");

            

            //GUI.setScreen(loginScreen);
            
            /*

            if (Network.connectToServer())
            {
                ScreenController.transitionToLoginScreen();
            }
            else
            {
                ScreenController.transitionToMainMenu();
            }
            */

        }

        public static void attemptLogin(string username, string password)
        {
            if (Network.login(username, password))
            {
                user = Network.queryMyUser();
                GUI.frame.loginAs(user);

                var friends = Network.queryFriends();
                user.setFriends(friends);

                var collection = Network.queryCollection();
                ownedCards = collection.ToList();

                var shekels = Network.queryShekels();
                setShekelBalance(shekels);

                var ownedPacks = Network.queryOwnedPacks();
                shopScreen.populate(ownedPacks);

                GUI.setScreen(shopScreen);
            }
            else
            {
                
            }
        }

        public static bool ripPack(Packs pack)
        {
            var ripped = Network.ripPack(pack);
            if (ripped == null) return false;
            else
            {
                shopScreen.ripPack(ripped);
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
            GUI.setScreen(gsc);
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
