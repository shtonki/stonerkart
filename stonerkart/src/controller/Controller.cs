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
        private static GameFrame gameFrame;

        private static HexPanel hexPanel => gameFrame.gamePanel.hexPanel;

        private static DraggablePanel friendsList;
        private static FriendsListPanel friendsListPanel = new FriendsListPanel();

        private static List<string> friends;

        public static bool inGame;

        public static void startup()
        {
            gameFrame = launchUI();
            if (Network.connectToServer())
            {
                gameFrame.transitionTo(gameFrame.loginPanel);
            }
            else
            {
                gameFrame.transitionTo(gameFrame.mainMenuPanel);
            }
        }

        public static void quit()
        {
            Settings.saveSettings();
            Environment.Exit(1);
        }

        public static Deck chooseDeck()
        {
            ManualResetEventSlim re = new ManualResetEventSlim(false);
            string s = null;
            Thread t = new Thread(() => chooseDeck(v =>
            {
                s = v;
                re.Set();
            }));
            t.Start();
            re.Wait();

            return loadDeck(s);
        }

        public static void chooseDeck(Action<string> cb)
        {
            Panel p = new Panel();
            p.BackColor = Color.Tomato;
            string[] decknames = Controller.getDeckNames();
            p.Size = new Size(100, 20 * decknames.Length);
            DraggablePanel dp = null;
            for (int i = 0; i < decknames.Length; i++)
            {
                string s = decknames[i];
                Button b = new Button();
                b.Text = s;
                b.Click += (_, __) =>
                {
                    cb(s);
                    dp.close();
                };
                p.Controls.Add(b);
                b.BackColor = Color.Violet;
                b.SetBounds(0, i * 20, p.Size.Width, 20);
            }

            dp = Controller.showControl(p, true, false);
        }

        public static void saveDeck(Deck d, string name)
        {
            using (FileStream f = File.Create(Settings.decksPath + name + ".jas"))
            {
                byte[] xd = Encoding.ASCII.GetBytes(d.toSaveText());
                f.Write(xd, 0, xd.Length);
            }
        }

        public static Deck loadDeck(string name)
        {
            using (StreamReader r = new StreamReader(File.Open(Settings.decksPath + name + ".jas", FileMode.Open)))
            {
                string s = r.ReadToEnd();
                return new Deck(s);
            }
        }

        public static string[] getDeckNames()
        {
            return Directory.EnumerateFiles(Settings.decksPath)
                .Where(s => s.EndsWith(".jas"))
                .Select(s => s.Substring(s.LastIndexOf('/') + 1, s.Length - 6))
                .ToArray();
        }

        public static void challengePlayer(string username)
        {
            Network.challenge(username);
        }

        public static void newGame(NewGameStruct ngs, bool local)
        {
            inGame = true;
            Game g = new Game(ngs, local);
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

        public static void transitionToDeckEditor()
        {
            gameFrame.transitionTo(gameFrame.deckEditorScreen);
        }

        public static void setFriendList(List<string> fs)
        {
            friends = new List<string>();

            foreach (var f in fs)
            {
                friends.Add(f);
                friendsListPanel.showFriend(f);
            }
            gameFrame.menuBar1.enableFriendsButton();
        }

        public static void addFriend(string name)
        {
            if (friends.Contains(name)) return;

            if (Network.addFriend(name))
            {
                friends.Add(name);
                friendsListPanel.showFriend(name);
            }
        }

        public static void toggleFriendsList()
        {
            if (friendsList != null && !friendsList.closed)
            {
                friendsList.close();
                friendsList = null;
            }
            else
            {
                friendsList = showControl(friendsListPanel);
            }
        }
        

        public static DraggablePanel showControl(Control c, bool resizeable = true, bool closeable = true)
        {
            return gameFrame.showControl(c, resizeable, closeable);
        }

        private static Dictionary<Pile, DraggablePanel> cachex = new Dictionary<Pile, DraggablePanel>();
        public static void toggleShowPile(Player p, PileLocation pl)
        {
            Pile pile = p.pileFrom(pl);
            if (cachex.ContainsKey(pile))
            {
                DraggablePanel v = cachex[pile];
                if (!v.closed)
                {
                    v.close();
                    cachex.Remove(pile);
                    return;
                }
            }
            DraggablePanel dp= showPile(pile, true, c => { });
            cachex[pile] = dp;
        }

        public static DraggablePanel showPile(Pile p, bool closeable, Action<Clickable> clicked)
        {
            CardsPanel cp = new CardsPanel(p);
            cp.clickedCallbacks.Add(clicked);
            return Controller.showControl(cp, true, closeable);
        }

        public static DraggablePanel showCards(IEnumerable<Card> cards, bool closeable, Action<Clickable> clicked)
        {
            CardsPanel p = new CardsPanel();
            p.clickedCallbacks.Add(clicked);
            p.setCards(cards);
            return Controller.showControl(p, true, closeable);
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

        public static void toggleOptionPanel()
        {
            gameFrame.toggleOptionPanel();
        }

        public static void setPrompt(string prompt, params ButtonOption[] options)
        {
            gameFrame.setPrompt(prompt, options);
        }

        private static List<TileView> hled = new List<TileView>();

        public static void clearHighlights(bool rd = true)
        {
            foreach (var v in hled) v.colour = Color.Black;
            hled.Clear();
            if (rd) redraw();
        }

        private static void highlight(TileView tv, Color c)
        {
            tv.colour = c;
            hled.Add(tv);
        }

        private static void highlight(Color c, params Tile[] tvs)
        {
            highlight(tvs.Select(v => new Tuple<Color, Tile>(c, v)));
        }

        public static void highlight(IEnumerable<Tile> tiles, Color color)
        {
            highlight(tiles.Select(t => new Tuple<Color, Tile>(color, t)));
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
            //gameFrame?.gamePanel?.consolePanel?.print(s);
        }
    }
}
