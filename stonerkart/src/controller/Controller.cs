using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace stonerkart
{
    /// <summary>
    /// 
    /// </summary>
    static class Controller
    {


        public static bool inGame;

        public static void startup()
        {
            UIController.launchUI();
            


            if (Network.connectToServer())
            {
                ScreenController.transitionToLoginScreen();
            }
            else
            {
                ScreenController.transitionToMainMenu();
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
