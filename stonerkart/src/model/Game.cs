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

        public readonly Pile stack = new Pile(new Location(null, PileLocation.Stack));

        public Player activePlayer => hero;

        private List<GameEventHandler> geFilters = new List<GameEventHandler>();
        private Stack<StackWrapper> wrapperStack = new Stack<StackWrapper>();

        private IEnumerable<Card> herpable => hero.field;

        public Game(Map map)
        {
            this.map = map;
            hero = new Player(this, CardTemplate.Hero);
            villain = new Player(this, CardTemplate.Hero);

            setupHandlers();
        }

        private void setupHandlers()
        {
            geFilters.Add(new GameEventHandler<PayCostsEvent>(e =>
            { 
                e.ability.cost.cut(e.player, e.costs);
            }));

            geFilters.Add(new GameEventHandler<DrawEvent>(e =>
            {
                e.player.deck.removeTop().moveTo(e.player.hand);
            }));

            geFilters.Add(new GameEventHandler<CastEvent>(e =>
            {
                Card c = e.wrapper.card;

                wrapperStack.Push(e.wrapper);
                c.moveTo(stack);

                Controller.redraw();
            }));

            geFilters.Add(new GameEventHandler<MoveToTileEvent>(e =>
            {
                e.card.moveTo(e.tile);
            }));

            geFilters.Add(new GameEventHandler<AttackEvent>(e =>
            {
                raiseEvent(new MoveToTileEvent(e.attacker, e.attackFrom));
                raiseEvent(new DamageEvent(e.attacker, e.defender, e.attacker.power));
                if (e.defender.cardType != CardType.Hero) raiseEvent(new DamageEvent(e.defender, e.attacker, e.defender.power));
            }));

            geFilters.Add(new GameEventHandler<DamageEvent>(e =>
            {
                e.target.toughness.modify(-e.amount, Modifiable.intAdd, Modifiable.startOfEndStep);
            }));

            geFilters.Add(new GameEventHandler<MoveToPileEvent>(e =>
            {
                e.card.moveTo(e.pile);
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
            map.tileAt(6, 6).place(villain.heroCard);
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
            activePlayer.resetMana();
            int[] r = activePlayer.stuntMana();
            Controller.setPrompt("Gain mana nerd");
            ManaOrb v = (ManaOrb)waitForButtonOr<ManaOrb>(o => r[(int)o.colour] != 6);
            activePlayer.unstuntMana(r, v.colour);
        }

        private void drawStep()
        {
            raiseEvent(new DrawEvent(activePlayer, 1));
        }

        private void mainStep()
        {
            while (true)
            {
                StackWrapper? v = getCast(activePlayer);
                if (v != null)
                {
                    StackWrapper w = v.Value;
                    raiseEvent(new PayCostsEvent(activePlayer, w.ability, w.costs));
                    raiseEvent(new CastEvent(w));
                }
                else //pass
                {
                    if (wrapperStack.Count == 0)
                    {
                        break;
                    }

                    StackWrapper wrapper = wrapperStack.Pop();
                    if (wrapper.card != stack.peekTop()) throw new Exception();
                    resolve(wrapper);

                    Controller.redraw();
                }
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

                var options = map.dijkstra(from).Where(too =>
                    too.length <= card.movement &&
                    too.to.enterableBy(card) &&
                    (too.to.card != null || !paths.Select(p => p.to).Contains(too.to))
                    ).ToList();

                Controller.highlight(options.Select(n => new Tuple<Color, Tile>(Color.Green, n.to)));
                var to = getTile(tile => tile == from || options.Select(p => p.to).Contains(tile), "Move to where?");
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
                    if (c.path.to.card == null)
                    {
                        raiseEvent(new MoveToTileEvent(c, c.path.to));
                    }
                    else
                    {
                        raiseEvent(new AttackEvent(c.path));
                    }
                    c.path = null;
                }
            }
        }

        private Card getCard(Func<Card, bool> f, string prompt)
        {
            Controller.setPrompt(prompt, "nigra");
            var v = waitForButtonOr<Card>(f);
            if (v is ShibbuttonStuff)
            {
                return null;
            }
            return (Card)v;
        }

        private Tile getTile(Func<Tile, bool> f, string prompt)
        {
            Controller.setPrompt(prompt, "nigra");
            var v = waitForButtonOr<Tile>(f);
            if (v is ShibbuttonStuff)
            {
                return null;
            }
            return (Tile)v;
        }

        private StackWrapper? getCast(Player p)
        {
            Tile from = p.heroCard.tile;
            while (true)
            {
                Card card = getCard((crd) => true, "Cast a card");
                if (card == null) return null;

                ActivatedAbility ability = chooseAbility(card);
                if (ability == null) continue;

                int[][] costs = ability.cost.measure(p);
                if (costs == null) continue;

                TargetMatrix[] targets = chooseCastTargets(ability, from);
                if (targets == null) continue;

                return new StackWrapper(card, ability, targets, costs);
            }
        }

        private ActivatedAbility chooseAbility(Card c)
        {
            ActivatedAbility[] v = c.usableHere;
            if (v.Length > 1) throw new Exception();
            if (v.Length == 0) return null;
            return v[0];
        }

        private TargetMatrix[] chooseCastTargets(Ability a, Tile castFrom)
        {
            TargetMatrix[] ms = new TargetMatrix[a.effects.Length];
            List<Tile> v = castFrom.withinDistance(a.castRange);

            Controller.highlight(v, Color.Green);
            Controller.setPrompt("target nigra", "Cancel");
            for (int i = 0; i < ms.Length; i++)
            {
                ms[i] = a.effects[i].ts.fillCast(generateTargetable(v));
            }
            Controller.clearHighlights();
            return ms;
        }
        

        private void resolve(StackWrapper wrapper)
        {
            TargetMatrix[] ts = wrapper.matricies;
            Effect[] es = wrapper.ability.effects;

            if (ts.Length != es.Length) throw new Exception();

            List<GameEvent> events = new List<GameEvent>();

            for (int i = 0; i < ts.Length; i++)
            {
                Effect effect = es[i];
                TargetMatrix matrix = effect.ts.fillResolve(ts[i], wrapper.card, this);

                events.AddRange(effect.doer.act(matrix.generateRows()));
            }

            foreach (GameEvent e in events)
            {
                raiseEvent(e);
            }

            raiseEvent(new MoveToPileEvent(wrapper.card.owner.field, wrapper.card));

        }
        
        private ManualResetEventSlim callerBacker;
        private InputEventFilter filter;
        private Stuff s;
        

        private Func<Targetable> generateTargetable(IEnumerable<Tile> allowedTiles)
        {
            Tile[] allowed = allowedTiles.ToArray();
            return () =>
            {
                while (true)
                {
                    Stuff v = waitFor(new InputEventFilter((clickable, o) => true));

                    if (v is Tile)
                    {
                        if (!allowed.Contains(v)) continue;
                        return (Tile)v;
                    }

                    if (v is Card)
                    {
                        return (Card)v;
                    }

                    if (v is ShibbuttonStuff)
                    {
                        return null;
                    }
                }
            };
        }

        public void clicked(Clickable c)
        {
            var stuff = c.getStuff();
            InputEvent e = new InputEvent(c, stuff);
            if (filter != null && filter.filter(e))
            {
                s = stuff;
                callerBacker.Set();
            }
        }

        private Stuff waitForButtonOr<T>(Func<T, bool> fn) where T : Stuff
        {
            InputEventFilter f = new InputEventFilter((clickable, o) => clickable is Shibbutton || (o is T && fn((T)o)));
            return waitFor(f);
        }
        
        private Stuff waitFor(InputEventFilter f)
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
    }

    struct StackWrapper
    {
        public readonly Card card;
        public readonly Ability ability;
        public readonly TargetMatrix[] matricies;
        public readonly int[][] costs;

        public StackWrapper(Card card, Ability ability, TargetMatrix[] matricies, int[][] costs)
        {
            this.card = card;
            this.ability = ability;
            this.matricies = matricies;
            this.costs = costs;
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
