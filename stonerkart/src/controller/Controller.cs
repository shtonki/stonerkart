using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace stonerkart
{
    /// <summary>
    /// Collection of functions for controlling the flow of the game, UI etc.
    /// </summary>
    static class Controller
    {
        private static GameFrame gameFrame;

        private static HexPanel hexPanel => gameFrame.gamePanel.hexPanel;

        public static void startup()
        {
            gameFrame = launchUI();
            gameFrame.transitionTo(gameFrame.mainMenuPanel);
        }

        public static void newGame()
        {
            Game g = new Game(new Map(21, 13, false, false));
            gameFrame.toGame(g);
            g.startGame();
        }

        public static void transitionToMapEditor()
        {
            gameFrame.transitionTo(gameFrame.mapEditorScreen);
        }

        public static void transitionToMainMenu()
        {
            gameFrame.transitionTo(gameFrame.mainMenuPanel);
        }

        public static void addArrow(List<Tile> l)
        {
            hexPanel.ts.Add(l);
        }

        public static void removeArrow(List<Tile> l)
        {
            hexPanel.ts.Remove(l);
        }

        public static void clearArrows()
        {
            hexPanel.ts.Clear();
            redraw();
        }

        private static GameFrame launchUI()
        {
            GameFrame r = new GameFrame();
            ManualResetEvent e = new ManualResetEvent(false);
            EventHandler a = (x, y) => e.Set();
            r.Load += a;
            Thread t = new Thread(() => Application.Run(r));
            t.Start();
            e.WaitOne();
            r.Load -= a;
            return r;
        }

        public static void setPrompt(string s, params string[] ss)
        {
            gameFrame.setPrompt(s, ss);
        }
        

        private static List<TileView> hled = new List<TileView>();
        public static Color? highlightColor;

        public static void clearHighlights(bool rd = true)
        {
            foreach (var v in hled) v.color = Color.Firebrick;
            hled.Clear();
            if (rd) redraw();
        }

        private static void highlight(TileView tv, Color c)
        {
            tv.color = c;
            hled.Add(tv);
        }

        private static void highlight(Color c, params Tile[] tvs)
        {
            highlight(tvs.Select(v => new Tuple<Color, Tile>(c, v)));
        }

        public static void highlight(IEnumerable<Tuple<Color, Tile>> l, bool rd = true)
        {
            foreach (var v in l)
            {
                highlight(hexPanel.viewOf(v.Item2), v.Item1);
            }
            if (rd) redraw();
        }

        public static void redraw()
        {
            Control v = gameFrame.gamePanel;
            //v.memeout(v.Refresh);
            v = gameFrame.gamePanel.hexPanel;
            v.memeout(v.Invalidate);
        }

        public static void print(string s)
        {
            gameFrame?.gamePanel?.consolePanel?.print(s);
        }
    }
}
