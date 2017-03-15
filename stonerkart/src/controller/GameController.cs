using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace stonerkart
{
    class GameController
    {
        private Game game;
        private GamePanel gamePanel;

        private Dictionary<Pile, DraggablePanel> cachex = new Dictionary<Pile, DraggablePanel>();

        private HexPanel hexPanel => gamePanel.hexPanel;

        public GameController(Game game, GamePanel gamePanel)
        {
            this.game = game;
            this.gamePanel = gamePanel;
        }

        public void toggleShowPile(Player p, PileLocation pl)
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
            DraggablePanel dp = showPile(pile, true, c => { game.clicked(c); });
            cachex[pile] = dp;
        }

        public DraggablePanel showPile(Pile p, bool closeable, Action<Clickable> clicked)
        {
            CardsPanel cp = new CardsPanel(p);
            cp.clickedCallbacks.Add(clicked);
            return UIController.showControl(cp, true, closeable);
        }

        public DraggablePanel showCards(IEnumerable<Card> cards, bool closeable, Action<Clickable> clicked)
        {
            CardsPanel p = new CardsPanel();
            p.clickedCallbacks.Add(clicked);
            p.setCards(cards);
            return UIController.showControl(p, true, closeable);
        }

        public void setHeroActive(bool b)
        {
            gamePanel.setHeroActive(b);
        }

        public void addArrow(List<Tile> l)
        {
            hexPanel.ts.Add(l);
        }

        public void removeArrow(List<Tile> l)
        {
            hexPanel.ts.Remove(l);
        }

        public void clearArrows()
        {
            hexPanel.ts.Clear();
            redraw();
        }

        public void setPrompt(string message, params ButtonOption[] buttons)
        {
            gamePanel.memeout(() =>
            {
                gamePanel.promtText.Text = message;
                Shibbutton[] bs = new[] {
                    gamePanel.shibbutton2,
                    gamePanel.shibbutton3,
                    gamePanel.shibbutton4,
                    gamePanel.shibbutton5,
                };
                for (int i = 0; i < bs.Length; i++)
                {
                    if (buttons.Length > i && buttons[i] != ButtonOption.NOTHING)
                    {
                        bs[i].Visible = true;
                        bs[i].setOption(buttons[i]);
                    }
                    else
                    {
                        bs[i].Visible = false;
                    }
                }
                gamePanel.Invalidate();
            });
        }

        private List<TileView> hled = new List<TileView>();

        public void clearHighlights(bool rd = true)
        {
            foreach (var v in hled) v.colour = Color.Black;
            hled.Clear();
            if (rd) redraw();
        }

        private void highlight(TileView tv, Color c)
        {
            tv.colour = c;
            hled.Add(tv);
        }

        private void highlight(Color c, params Tile[] tvs)
        {
            highlight(tvs.Select(v => new Tuple<Color, Tile>(c, v)));
        }

        public void highlight(IEnumerable<Tile> tiles, Color color)
        {
            highlight(tiles.Select(t => new Tuple<Color, Tile>(color, t)));
        }

        public void highlight(IEnumerable<Tuple<Color, Tile>> l, bool rd = true)
        {
            foreach (var v in l)
            {
                highlight(hexPanel.viewOf(v.Item2), v.Item1);
            }
            if (rd) redraw();
        }

        public void redraw()
        {
            Control v = gamePanel;
            //v.memeout(v.Refresh);
            v = gamePanel.hexPanel;
            v.memeout(v.Invalidate);
        }
    }
}
