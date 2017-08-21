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
            GUI.launch();
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
