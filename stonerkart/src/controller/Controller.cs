using System.Windows.Forms;

namespace stonerkart
{
    static class Controller
    {
        private static GameFrame gameFrame;

        #region Control
        public static void start()
        {
            gameFrame = new GameFrame();
            Application.Run(gameFrame);
        }
        #endregion

        #region Game
        public static void clicked(Tile t)
        {
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
