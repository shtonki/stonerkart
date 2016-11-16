using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;


namespace stonerkart
{
    class Game
    {
        public readonly Map map;
        public readonly Player hero;
        public readonly Player villain;

        private List<GameEventHandler> geFilters = new List<GameEventHandler>();

        public Game(Map map)
        {
            this.map = map;
            hero = new Player();

            geFilters.Add(new GameEventHandler<DrawEvent>(
                (de) => de.player.deck.draw().moveTo(de.player.hand)
                ));
        }

        private void raiseEvent(GameEvent e)
        {
            foreach (var v in geFilters)
            {
                v.handle(e);
            }
        }

        public void startGame()
        {
            Thread t = new Thread(loopEx);
            t.Start();
        }

        private void loopEx()
        {
            while (true)
            {
                gameLoop();
            }
        }

        private void gameLoop()
        {
            #region upkeep

            #endregion

            #region draw
            raiseEvent(new DrawEvent(hero, 1));
            #endregion

            #region main1

            while (true)
            {
                Controller.setPrompt("Play a card?", "No");
                var v = Controller.waitForButtonOr<Card>();
                if (v is ShibbuttonStuff)
                {
                    break;
                }
                if (v is Card)
                {
                    Controller.setPrompt("Play to what tile?");
                    var o = Controller.waitForButtonOr<Tile>();
                    Tile t = (Tile)o;
                    Card c = (Card)v;
                    c.moveTo(hero.field);
                    t.place((Card)v);
                    Controller.setPrompt("");
                }
                else
                {
                    throw new ConstraintException();
                }
            }

            #endregion

            #region move

            while (true)
            {
                Controller.setPrompt("Move a card?", "No");
                var v = Controller.waitForButtonOr<Tile>();
                if (v is ShibbuttonStuff)
                {
                    break;
                }
                if (v is Tile)
                {
                    Tile from = (Tile)v;
                    if (from.card != null)
                    {
                        var ns = from.withinDistance(2);
                        Controller.highlight(ns.Select(n => new Tuple<Color, Tile>(Color.Green, n)));
                        Controller.setPrompt("Move to what tile?");
                        Tile moveTo = null;
                        while (true)
                        {
                            var o = Controller.waitForButtonOr<Tile>();
                            if (o is ShibbuttonStuff)
                            {
                                break;
                            }
                            Tile to = (Tile)o;
                            if (to.card == null && ns.Contains(to))
                            {
                                moveTo = to;
                                break;
                            }
                        }
                        if (moveTo != null)
                        {
                            moveTo.place(from.removeCard());
                        }
                        Controller.setPrompt("");
                        Controller.clearHighlights();
                    }
                }
                else
                {
                    throw new ConstraintException();
                }
            }

            #endregion
        }
    }
}
