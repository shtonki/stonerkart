using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace stonerkart
{
    /// <summary>
    /// Collection of functions for controlling the flow of the game, UI etc.
    /// </summary>
    static class Controller
    {
        private static GameFrame gameFrame;
        public static void start(Map m)
        {

            G.load(m);
            gameFrame = new GameFrame();
            G.unload();

            Application.Run(gameFrame);
        }
        
        #region GUI
        public static void redraw()
        {
            gameFrame?.gamePanel?.hexPanel?.Refresh();
        }
        #endregion

        #region Console

        public static void print(string s)
        {
            gameFrame?.gamePanel?.consolePanel?.print(s);
        }
        #endregion
    }
}
