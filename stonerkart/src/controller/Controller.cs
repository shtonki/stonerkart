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

        public static void mouseEntered(TileView tv)
        {
        }


        public static void clicked(Clickable c)
        {
            var stuff = c.getStuff();
            InputEvent e = new InputEvent(c, stuff);
            if (filter != null && filter.filter(e))
            {
                s = stuff;
                callerBacker.Set();
            }
            if (c is TileView)
            {
                Tile t = (Tile)stuff;
            }
        }
    
        private static ManualResetEventSlim callerBacker;
        private static InputEventFilter filter;
        private static Stuff s;

        public static Stuff waitForButtonOr<T>(Func<T, bool> fn) where T : Stuff
        {
            InputEventFilter f = new InputEventFilter((clickable, o) => clickable is Shibbutton || (o is T && fn((T)o)));
            return waitFor(f);
        }

        public static Stuff waitForButtonOr<T>() where T : Stuff
        {
            InputEventFilter f = new InputEventFilter((clickable, o) => clickable is Shibbutton || o is T);
            return waitFor(f);
        }

        public static T waitFor<T>() where T : Stuff
        {
            InputEventFilter f = new InputEventFilter((clickable, o) => o is T);
            return (T)waitFor(f);
        }

        private static Stuff waitFor(InputEventFilter f)
        {
            if (callerBacker != null || filter != null) throw new Exception();
            callerBacker = new ManualResetEventSlim();
            filter = f;
            callerBacker.Wait();
            var r = s;
            s = null;
            callerBacker = null;
            filter = null;
            return r;
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

        private static TileView getView(Tile t)
        {
            return gameFrame.gamePanel.hexPanel.viewOf(t);
        }
    }
}
