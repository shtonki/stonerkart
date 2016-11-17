using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
//using hackery = System.Tuple<>;


namespace stonerkart
{
    class Game
    {
        public readonly Map map;
        public readonly Player hero;
        public readonly Player villain;

        public Player activePlayer => hero;

        private List<GameEventHandler> geFilters = new List<GameEventHandler>();

        public Game(Map map)
        {
            this.map = map;
            Card herpo = new Card(CardTemplate.Hero);
            hero = new Player(herpo);

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

        private void init()
        {
            map.tileAt(4, 4).place(hero.heroCard);
            hero.field.add(hero.heroCard);
        }

        private void loopEx()
        {
            init();

            while (true)
            {
                gameLoop();
            }
        }

        private void gameLoop()
        {

            upkeepStep();

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
                    var ns = hero.heroCard.tile.withinDistance(2);
                    Controller.highlight(ns.Select(n => new Tuple<Color, Tile>(Color.Green, n)));
                    var o = Controller.waitForButtonOr<Tile>(tile => tile.card == null && ns.Contains(tile));
                    Tile t = (Tile)o;
                    Card c = (Card)v;
                    c.moveTo(hero.field);
                    t.place((Card)v);
                    Controller.setPrompt("");
                    Controller.clearHighlights();
                }
                else
                {
                    throw new ConstraintException();
                }
            }

            #endregion

            moveStep();
        }

        private void upkeepStep()
        {
            foreach (Card c in activePlayer.field)
            {
                c.movement = c.baseMovement;
            }
        }

        private void moveStep()
        {
            while (true)
            {
                Controller.setPrompt("Move a card?", "No");
                var v = Controller.waitForButtonOr<Tile>(tile => tile.card.movement > 0);
                if (v is ShibbuttonStuff)
                {
                    break;
                }
                Tile from = (Tile)v;
                if (from.card != null)
                {
                    Card card = from.card;

                    if (card.path != null)
                    {
                        Controller.removeArrow(card.tile, card.path);
                        card.path = null;
                    }

                    var ns = from.withinDistance(card.movement, 0);
                    Controller.highlight(ns.Select(n => new Tuple<Color, Tile>(Color.Green, n)));
                    Controller.setPrompt("Move to what tile?");
                    var o =
                        Controller.waitForButtonOr<Tile>(
                            tile => (tile.card == null || tile == from) && ns.Contains(tile));
                    if (o is ShibbuttonStuff)
                    {
                        break;
                    }
                    Tile to = (Tile)o;
                    if (to != from)
                    {
                        Controller.addArrow(from, to);
                        card.path = to;
                        /*
                        
                        */
                    }
                    Controller.setPrompt("");
                    Controller.clearHighlights();
                }
            }
            Controller.clearArrows();
            foreach (Card c in activePlayer.field)
            {
                if (c.path != null)
                {
                    c.tile.removeCard();
                    c.path.place(c);
                    c.movement = 0;
                }
                c.path = null;
            }
        }
    }
}
