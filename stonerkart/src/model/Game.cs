﻿using System;
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

        public Player activePlayer => hero;

        private List<GameEventHandler> geFilters = new List<GameEventHandler>();

        private IEnumerable<Card> herpable => hero.field;

        public Game(Map map)
        {
            this.map = map;
            Card herpo = new Card(CardTemplate.Hero);
            hero = new Player(herpo);

            setupHandlers();
        }

        private void setupHandlers()
        {
            geFilters.Add(new GameEventHandler<DrawEvent>(e =>
            {
                e.player.deck.draw().moveTo(e.player.hand);
            }));

            geFilters.Add(new GameEventHandler<CastEvent>(e =>
            {
                Card c = e.wrapper.card;
                Tile t = e.wrapper.tile;
                c.moveTo(hero.field);
                c.moveTo(t);
                Controller.redraw();
            }));

            geFilters.Add(new GameEventHandler<MoveEvent>(e =>
            {
                e.card.moveTo(e.tile);
            }));
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
                var v = getCast();
                if (v == null) break;
                raiseEvent(new CastEvent(v.Value));
            }
        }

        private void moveStep()
        {
            List<Path> paths = new List<Path>();
            while (true)
            {
                var from = getTile(tile => tile.card != null && tile.card.movement > 0, "Move your cards nigra");
                if (from == null) break;

                Card card = from.card;

                if (card.path != null)
                {
                    Controller.removeArrow(card.path);
                    paths.Remove(card.path);
                    card.path = null;
                }

                var options = map.dijkstra(from).Where(t =>
                    t.length <= card.movement &&
                    t.to.card == null &&
                    !paths.Select(p => p.to).Contains(t.to)
                    ).ToList();

                Controller.highlight(options.Select(n => new Tuple<Color, Tile>(Color.Green, n.to)));
                var to = getTile(tile => tile == from || (tile.card == null && options.Select(p => p.to).Contains(tile)), "Move to where?");
                Controller.clearHighlights();
                if (to == null) break;
                
                if (to != from)
                {
                    var path = options.First(p => p.to == to);
                    Controller.addArrow(path);
                    card.path = path;
                    paths.Add(path);
                }
            }

            Controller.clearArrows();

            foreach (Card c in activePlayer.field)
            {
                if (c.path != null)
                {
                    raiseEvent(new MoveEvent(c, c.path.to));
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

        private Tile getTile(Func<Tile, bool> f, string prompt)
        {
            Controller.setPrompt(prompt, "nigra");
            var v = Controller.waitForButtonOr<Tile>(f);
            if (v is ShibbuttonStuff)
            {
                return null;
            }
            return (Tile)v;
        }

        private StackWrapper? getCast()
        {
            while (true)
            {
                Card c = getCard((crd) => true, "Cast a card");
                if (c == null) return null;

                var ns = hero.heroCard.tile.withinDistance(2);
                Controller.highlight(ns.Select(n => new Tuple<Color, Tile>(Color.Green, n)));
                Tile t = getTile(tile => tile.card == null && ns.Contains(tile), "To what tile");
                Controller.clearHighlights();
                if (t == null) continue;

                return new StackWrapper(c, t);
            }
        }
    }

    struct StackWrapper
    {
        public readonly Card card;
        public readonly Tile tile;

        public StackWrapper(Card card, Tile tile)
        {
            this.card = card;
            this.tile = tile;
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
