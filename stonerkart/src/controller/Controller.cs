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
        public static void launchGame()
        {
            if (!Network.connectToServer()) throw new Exception("Serber offline");

            GUI.launch();

            GUI.setScreen(new GameScreen());

            Game g = new Game(new NewGameStruct(0, 0, new []{"a", "b"}, 0), true);
            g.startGame();

            //throw new NotImplementedException("if you weren't expecting too see this you might be in some trouble son");
            //UIController.launchUI();
            
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
                GUI.setScreen(new GameScreen());
            }
            else
            {
                
            }
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
