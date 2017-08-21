﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace stonerkart
{
    class GameController
    {
        private Game game;
        private GameScreen gameScreen;

        private Dictionary<Pile, DraggablePanel> cachex = new Dictionary<Pile, DraggablePanel>();

        private HexPanel hexPanel => null;//gameScreen.hexPanel;

        public GameController(Game game, GameScreen gameScreen)
        {
            this.game = game;
            this.gameScreen = gameScreen;
        }

        public void toggleShowPile(Player p, PileLocation pl)
        {
            throw new NotImplementedException("if you weren't expecting too see this you might be in some trouble son");
            /*
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
            */
        }

        public DraggablePanel showPile(Pile p, bool closeable, Action<Clickable> clicked)
        {

            throw new NotImplementedException("if you weren't expecting too see this you might be in some trouble son");
            /*
            CardsPanel cp = new CardsPanel(p);
            cp.clickedCallbacks.Add(clicked);
            return UIController.showControl(cp, true, closeable);
            */
        }

        public DraggablePanel showCards(IEnumerable<Card> cards, bool closeable, Action<Clickable> clicked)
        {

            throw new NotImplementedException("if you weren't expecting too see this you might be in some trouble son");
            /*
            CardsPanel p = new CardsPanel();
            p.clickedCallbacks.Add(clicked);
            p.setCards(cards);
            return UIController.showControl(p, true, closeable);
            */
        }

        public void setHeroActive(bool b)
        {

            throw new NotImplementedException("if you weren't expecting too see this you might be in some trouble son");/*
            gameScreen.setHeroActive(b);
            */
        }

        public void addArrow(Path l)
        {

            throw new NotImplementedException("if you weren't expecting too see this you might be in some trouble son");
            //hexPanel.paths.Add(l);
        }

        public void removeArrow(Path l)
        {

            throw new NotImplementedException("if you weren't expecting too see this you might be in some trouble son");
            //hexPanel.paths.Remove(l);
        }

        public void clearArrows()
        {

            throw new NotImplementedException("if you weren't expecting too see this you might be in some trouble son");
            /*
            hexPanel.paths.Clear();
            redraw();
            */
        }

        public void setPrompt(string message, params ButtonOption[] buttons)
        {
            return;
            throw new NotImplementedException("if you weren't expecting too see this you might be in some trouble son");
            /*
            gameScreen.memeout(() =>
            {
                gameScreen.promtText.Text = message;
                Shibbutton[] bs = new[] {
                    gameScreen.shibbutton2,
                    gameScreen.shibbutton3,
                    gameScreen.shibbutton4,
                    gameScreen.shibbutton5,
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
                gameScreen.Invalidate();
            });
            */
        }

        //private List<TileView> hled = new List<TileView>();

        public void clearHighlights(bool rd = true)
        {

            throw new NotImplementedException("if you weren't expecting too see this you might be in some trouble son");/*
            foreach (var v in hled) v.colour = Color.Black;
            hled.Clear();
            if (rd) redraw();
            */
        }

        private void highlight(/*TileView tv, */Color c)
        {

            throw new NotImplementedException("if you weren't expecting too see this you might be in some trouble son");/*
            tv.colour = c;
            hled.Add(tv);
            */
        }

        private void highlight(Color c, params Tile[] tvs)
        {
            highlight(tvs.Select(v => new Tuple<Color, Tile>(c, v)));
        }

        public void highlight(IEnumerable<Tile> tiles, Color color)
        {
            highlight(tiles.Select(t => new Tuple<Color, Tile>(color, t)));
        }

        private  void highlight(IEnumerable<Tuple<Color, Tile>> l, bool rd = true)
        {

            throw new NotImplementedException("if you weren't expecting too see this you might be in some trouble son");/*
            foreach (var v in l)
            {
                highlight(hexPanel.viewOf(v.Item2), v.Item1);
            }
            if (rd) redraw();
            */
        }
    }
}
