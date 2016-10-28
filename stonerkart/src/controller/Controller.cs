using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace stonerkart
{
    static class Controller
    {
        private static GameFrame gameFrame;
        private static HexPanel hexPanel;
        private static Map map;

        #region Control
        public static void start(Map m)
        {
            map = m;

            G.load(map);
            gameFrame = new GameFrame();
            G.unload();

            hexPanel = gameFrame.gamePanel.hexPanel;

            Application.Run(gameFrame);
        }
        #endregion

        #region Game
        public static void clicked(TileView tv)
        {
        }

        private static List<TileView> lit = new List<TileView>();
        public static void entered(TileView tv)
        {
            foreach (var x in lit)
            {
                x.highlight = false;
            }
            lit.Clear();
            Tile t = tv.tile;
            Tile[] tx = map.neighboursOf(t);
            foreach (Tile x in tx.Concat(new Tile[] {t}))
            {
                if (x == null) continue;
                var v = hexPanel.viewOf(x);
                lit.Add(v);
                v.highlight = true;
            }
            redraw();
        }
        #endregion

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
