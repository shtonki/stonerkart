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

        public static void launchGame()
        {
            /*
            GUI.launch();
            var gsc = new GameScreen();
            GUI.setScreen(gsc);
            Game g = new Game(new NewGameStruct(0, 420, new[] { "Hero", "Villain" }, 0), true, gsc);
            g.startGameThread();
            return;
            */ 

            if (!Network.connectToServer()) throw new Exception("Serber offline");

            GUI.launch();

            GUI.setScreen(loginScreen);

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
                GUI.setScreen(mainMenuScreen);
            }
            else
            {
                
            }
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
