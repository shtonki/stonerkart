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

        public Player activePlayer => hero;

        private List<GameEventHandler> geFilters = new List<GameEventHandler>();

        private IEnumerable<Card> herpable => hero.field;

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

            foreach (var v in herpable)
            {
                v.reherp(e);
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
            untapStep();
            drawStep();
            mainStep();
            moveStep();
        }

        private void untapStep()
        {
            raiseEvent(new StartOfStepEvent(Steps.Untap));
        }

        private void drawStep()
        {
            raiseEvent(new DrawEvent(hero, 1));
        }

        private void mainStep()
        {
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
        }

        private void moveStep()
        {
            List<List<Tile>> paths = new List<List<Tile>>();
            while (true)
            {
                Controller.setPrompt("Move a card?", "No");
                var v = Controller.waitForButtonOr<Tile>(tile => tile.card != null && tile.card.movement > 0);

                if (v is ShibbuttonStuff)
                {
                    break;
                }

                Tile from = (Tile)v;
                Card card = from.card;

                var options = from.withinDistance(card.movement, 1).Where(possibleDestination => 
                  possibleDestination.card == null && 
                  (card.path?.Last() == possibleDestination || paths.Count(l => l.Last() == possibleDestination) == 0) &&
                  map.path(from, possibleDestination).Count - 1 <= card.movement
                ).ToArray();

                Controller.highlight(options.Select(n => new Tuple<Color, Tile>(Color.Green, n)));
                Controller.setPrompt("Move to what tile?");
                var o = Controller.waitForButtonOr<Tile>(tile => tile == from || (tile.card == null && options.Contains(tile)));

                if (o is ShibbuttonStuff)
                {
                    break;
                }

                Tile to = (Tile)o;
                if (card.path != null)
                {
                    Controller.removeArrow(card.path);
                    paths.Remove(card.path);
                    card.path = null;
                }
                if (to != from)
                {
                    var path = map.path(from, to);
                    Controller.addArrow(path);
                    card.path = path;
                    paths.Add(path);
                }

                Controller.setPrompt("");
                Controller.clearHighlights();
            }

            Controller.clearArrows();

            foreach (Card c in activePlayer.field)
            {
                if (c.path != null)
                {
                    c.tile.removeCard();
                    c.path.Last().place(c);
                    c.movement.modify(1-c.path.Count, Modifiable.intAdd, new TypedGameEventFilter<StartOfStepEvent>(e => e.step == Steps.Untap));
                    c.path = null;
                }
            }
        }

        private Card getCard(Func<Card, bool> f, string prompt)
        {
            Controller.setPrompt(prompt, "nigra");
            var v = Controller.waitForButtonOr<Card>(f);
            if (v is ShibbuttonStuff)
            {
                return null;
            }
            return (Card)v;
        }

        private StackWrapper? getCast()
        {
            Card c = getCard((crd) => true, "xd");
            if (c == null) return null; 

            return new StackWrapper(c); ;
        }
    }

    struct StackWrapper
    {
        public readonly Card card;

        public StackWrapper(Card card)
        {
            this.card = card;
        }
    }

    enum Steps
    {
        Untap,
        Draw,
        Main1,
        Combat,
        Damage,
        Main2,
        End
    }
}
