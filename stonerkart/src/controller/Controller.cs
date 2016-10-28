using System.Windows.Forms;

namespace stonerkart
{
    static class Controller
    {
        private static GameFrame gameFrame;
        private static Map m;

        #region Control
        public static void start(Map m)
        {
            gameFrame = new GameFrame();
            Application.Run(gameFrame);
        }
        #endregion

        #region Game
        public static void clicked(TileView t)
        {
            //Console.WriteLine(t.tile.a, t.tile.b, t.tile.c);
            //Console.WriteLine(t.tile.x, t.tile.y);
        }

        private static TileView en;
        public static void entered(TileView t)
        {
            if (en != null) en.highlighted = false;
            en = t;
            if (en != null) en.highlighted = true;
            redraw();
        }
        #endregion

        #region GUI

        public static void redraw()
        {
            gameFrame?.gamePanel?.hexPanel1?.Refresh();
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
