﻿
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace stonerkart
{
    class Game
    {
        public Map map { get; }
        public Player hero { get; }
        public Player villain { get; }

        private List<Player> players;
        private List<Card> cards;

        public Pile stack = new Pile(new Location(null, PileLocation.Stack));

        private int activePlayerIndex;
        public Player activePlayer => players[activePlayerIndex];

        public StepHandler stepHandler;

        private List<GameEventHandler> geFilters = new List<GameEventHandler>();
        private Stack<StackWrapper> wrapperStack = new Stack<StackWrapper>();

        private IEnumerable<Card> herpable => hero.field;

        private Random random;
        private GameConnection connection;

        public Game(NewGameStruct ngs, bool local)
        {
            if (local)
            {
                connection = new DummyConnection();
            }
            else
            {
                connection = new MultiplayerConnection(this, ngs);
            }

            random = new Random(ngs.randomSeed);
            map = new Map(21, 13, false, false);
            cards = new List<Card>();
            players = new List<Player>();

            for (int i = 0; i < ngs.playerNames.Length; i++)
            {
                Player p = new Player(this);
                Card heroCard = createCard(CardTemplate.Hero, p);
                p.setHeroCard(heroCard);

                players.Add(p);
                if (i == ngs.heroIndex) hero = p;
                else
                {
                    if (villain != null) throw new Exception();
                    villain = p;
                }

                List<Card> deck = new List<Card>();
                for (int j = 0; j < 10; j++)
                {
                    Card c = createCard(CardTemplate.Jordan, p);
                    deck.Add(c);
                }
                p.loadDeck(deck);
            }

            stepHandler = new StepHandler();

            setupHandlers();
        }

        public Card createCard(CardTemplate ct, Player owner)
        {
            Card r = new Card(ct, owner);
            cards.Add(r);
            return r;
        }

        public int ord(Card c)
        {
            return cards.IndexOf(c);
        }

        public Card cardFromOrd(int i)
        {
            return cards[i];
        }

        public int ord(Player p)
        {
            return players.IndexOf(p);
        }

        public Player playerFromOrd(int i)
        {
            return players[i];
        }

        public int ord(Tile t)
        {
            return map.ord(t);
        }

        public Tile tileFromOrd(int i)
        {
            return map.tileAt(i);
        }

        public Path pathTo(Card c, Tile t)
        {
            return map.path(c.tile, t);
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

        private void initGame()
        {
            activePlayerIndex = 0;
            map.tileAt(4, 4).place(players[0].heroCard);
            map.tileAt(6, 6).place(players[1].heroCard);

            foreach (Player p in players)
            {
                p.deck.shuffle(random);
            }
        }

        private void loopEx()
        {
            Controller.setPrompt("Game starting");
            initGame();

            while (true)
            {
                doStep(stepHandler.step);
                stepHandler.nextStep();
            }
        }

        private void doStep(Steps step)
        {
            switch (step)
            {
                case Steps.Untap:
                {
                    untapStep();
                } break;

                case Steps.Draw:
                {
                    drawStep();
                } break;

                case Steps.Main1:
                {
                    mainStep();
                } break;

                case Steps.Move:
                {
                    moveStep();
                } break;
                    
                case Steps.Main2:
                {
                    mainStep();
                } break;

                case Steps.End:
                {
                    endStep();
                } break;
            }
        }

        private void untapStep()
        {
            raiseEvent(new StartOfStepEvent(Steps.Untap));
            activePlayer.resetMana();

            ManaOrbSelection selection;

            if (activePlayer == hero)
            {

                ManaSet r = activePlayer.stuntMana();
                Controller.setPrompt("Gain mana nerd");
                ManaOrb v = (ManaOrb)waitForButtonOr<ManaOrb>(o => r[(int)o.colour] != 6);
                activePlayer.unstuntMana(r);

                selection = new ManaOrbSelection(v.colour);
                connection.sendAction(selection);
            }
            else
            {
                Controller.setPrompt("Opponent is gaining mama");
                selection = connection.receiveAction<ManaOrbSelection>();
            }

            activePlayer.gainMana(selection.orb);
        }

        private void drawStep()
        {
            raiseEvent(new DrawEvent(activePlayer, 1));
        }

        private void mainStep()
        {
            priority();
        }

        private void moveStep()
        {
            List<Tuple<Card, Path>> paths;

            if (activePlayer == hero)
            {
                paths = new List<Tuple<Card, Path>>();

                while (true)
                {
                    var from = getTile(tile => tile.card != null && tile.card.movement > 0 && tile.card.owner == activePlayer ,  "Move your cards nigra");
                    if (from == null) break;

                    Card card = from.card;
                    var tpl = paths.Find(tuple => tuple.Item1 == card);

                    if (tpl != null)
                    {
                        Controller.removeArrow(tpl.Item2);
                        paths.Remove(tpl);
                    }

                    var options = map.dijkstra(from).Where(too =>
                        too.length <= card.movement &&
                        too.to.enterableBy(card) &&
                        (too.to.card != null || !paths.Select(p => p.Item2.to).Contains(too.to))
                        ).ToList();

                    Controller.highlight(options.Select(n => new Tuple<Color, Tile>(Color.Green, n.to)));
                    var to = getTile(tile => tile == from || options.Select(p => p.to).Contains(tile), "Move to where?");
                    Controller.clearHighlights();
                    if (to == null) break;

                    if (to != from)
                    {
                        var path = options.First(p => p.to == to);
                        Controller.addArrow(path);
                        paths.Add(new Tuple<Card, Path>(card, path));
                    }
                }
                Controller.clearArrows();

                connection.sendAction(new MoveSelection(paths));
            }
            else
            {
                Controller.setPrompt("Opponent is moving");
                MoveSelection v = connection.receiveAction<MoveSelection>();
                paths = v.moves;
            }

            foreach (var v in paths)
            {
                Card c = v.Item1;
                Path p = v.Item2;
                if (p.to.card == null)
                {
                    raiseEvent(new MoveToTileEvent(c, p.to));
                }
                else
                {
                    raiseEvent(new AttackEvent(p));
                }
            }
            Controller.redraw();
        }

        private void endStep()
        {
            activePlayerIndex = (activePlayerIndex + 1)%players.Count;
        }

        private void enforceRules()
        {
            foreach (Card c in herpable)
            {
                if (c.toughness <= 0)
                {
                    raiseEvent(new MoveToPileEvent(c, c.owner.graveyard));
                }
            }
        }

        private void priority()
        {
            int c = 0;
            while (true)
            {
                Player fuckboy = players[(activePlayerIndex + c)%players.Count];
                StackWrapper? w = getCast(fuckboy);

                if (w.HasValue)
                {
                    raiseEvent(new PayCostsEvent(activePlayer, w.Value.ability, w.Value.costs));
                    raiseEvent(new CastEvent(w.Value));
                    c = 0;
                }
                else //pass
                {
                    c++;
                    if (c == players.Count)
                    {
                        if (wrapperStack.Count == 0)
                        {
                            break;
                        }

                        StackWrapper wrapper = wrapperStack.Pop();
                        if (wrapper.card != stack.peekTop()) throw new Exception();
                        resolve(wrapper);
                        c = 0;

                        Controller.redraw();
                    }
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
            StackWrapper? r;
            if (p == hero)
            {
                Tile from = p.heroCard.tile;
                while (true)
                {
                    Card card = getCard((crd) => true, "Cast a card");
                    if (card == null)
                    {
                        r = null;
                        break;
                    }

                    ActivatedAbility ability = chooseAbility(card);
                    if (ability == null) continue;

                    int[][] costs = ability.cost.measure(p);
                    if (costs == null) continue;

                    TargetMatrix[] targets = chooseCastTargets(ability, from);
                    if (targets == null) continue;

                    r = new StackWrapper(card, ability, targets, costs);

                    break;
                }
                connection.sendAction(new CastSelection(r));
            }
            else
            {
                Controller.setPrompt("Opponents turn to act.");
                r = connection.receiveAction<CastSelection>().wrapper;
            }

            return r;
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

            raiseEvent(new MoveToPileEvent(wrapper.card, wrapper.card.owner.field));

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

    class StepHandler : Observable<Steps>
    {
        public Steps step { get; private set; }

        public void nextStep()
        {
            if (step == Steps.End) step = Steps.Untap;
            else step = (Steps)(((int)step)+1);
            notify(step);
        }
    }

    enum Steps
    {
        Untap,
        Draw,
        Main1,
        Move,
        Main2,
        End
    }
}
