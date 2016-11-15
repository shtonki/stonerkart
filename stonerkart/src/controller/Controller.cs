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
        public static void startup()
        {
            gameFrame = launchUI();
            gameFrame.transitionTo(gameFrame.mainMenuPanel);
        }

        public static void newGame()
        {
            Game g = new Game(new Map(10, 10, false, false));
            gameFrame.toGame(g);
            g.startGame();
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

        public static void clicked(Clickable c)
        {
            if (callerBacker != null && callerBacker.filter.filter(c))
            {
                callerBacker.val = c.getStuff();
                callerBacker.e.Set();
                return;
            }

            if (debugFilter.filter(c))
            {
                System.Console.WriteLine(c.GetType());
                System.Console.WriteLine(c.getStuff().GetType());
                System.Console.WriteLine(c.getStuff());
                System.Console.WriteLine();
            }
            
        }
        private static ClickableFilter debugFilter = new ClickableFilter(
            new Type[] { },
            new Type[] { }
            );
        class CallerBacker
        {
            public readonly ClickableFilter filter;
            public readonly ManualResetEventSlim e;
            public object val;

            public CallerBacker(ClickableFilter filter)
            {
                this.filter = filter;
                e = new ManualResetEventSlim(false);
            }
        }

        private static CallerBacker callerBacker;

        public static object waitFor(ClickableFilter f)
        {
            if (callerBacker != null) throw new Exception();
            callerBacker = new CallerBacker(f);
            callerBacker.e.Wait();
            var r = callerBacker.val;
            callerBacker = null;
            return r;
        }

        public static void redraw()
        {
            var v = gameFrame.gamePanel;
            v.memeout(v.Refresh);
        }


        public static void print(string s)
        {
            gameFrame?.gamePanel?.consolePanel?.print(s);
        }
    }
}
