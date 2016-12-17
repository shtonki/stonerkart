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


        private IEnumerable<Card> triggerableCards => cards.Where(card => card.location.pile != PileLocation.Deck);

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
                Card heroCard = createCard(CardTemplate.Belwas, p, p.field);
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
                    Card c = createCard(CardTemplate.Cantrip, p, p.deck);
                }
                p.loadDeck(deck);
                p.deck.shuffle(random);
            }

            stepHandler = new StepHandler();

            setupHandlers();
        }

        public Card createCard(CardTemplate ct, Player owner, Pile pile)
        {
            Card r = new Card(ct, owner);
            cards.Add(r);
            r.moveTo(pile);
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
            geFilters.Add(new GameEventHandler<PlaceOnTileEvent>(e =>
            {
                if (e.tile.card != null) throw new Exception();

                e.card.moveTo(e.tile);
                e.card.moveTo(e.card.controller.field);
                e.card.exhaust();
            }));

            geFilters.Add(new GameEventHandler<PayCostsEvent>(e =>
            { 
                e.ability.cost.cut(e.player, e.costs);
            }));

            geFilters.Add(new GameEventHandler<DrawEvent>(e =>
            {
                e.player.deck.peek().moveTo(e.player.hand);
            }));

            geFilters.Add(new GameEventHandler<CastEvent>(e =>
            {
                Card c = e.wrapper.card;

                wrapperStack.Push(e.wrapper);
                c.moveTo(stack);

                Controller.redraw();
            }));

            geFilters.Add(new GameEventHandler<MoveEvent>(e =>
            {
                Path path = e.path;
                Card mover = path.from.card;
                Tile destination = path.to;
                Card defender = destination.card;

                if (defender == null)
                {
                    mover.moveTo(destination);
                    mover.movement.modify(-path.length, ModifiableSchmoo.intAdd, ModifiableSchmoo.startOfOwnersTurn(mover));
                }
                else
                {
                    if (path.penultimate.card != null && path.penultimate.card != mover) throw new Exception();

                    mover.moveTo(path.penultimate);
                    raiseEvent(new DamageEvent(mover, defender, mover.power));
                    if (!defender.isHeroic) raiseEvent(new DamageEvent(defender, mover, defender.power));
                    mover.exhaust();
                }

            }));

            geFilters.Add(new GameEventHandler<DamageEvent>(e =>
            {
                e.target.toughness.modify(-e.amount, ModifiableSchmoo.intAdd, ModifiableSchmoo.startOfEndStep);
            }));

            geFilters.Add(new GameEventHandler<MoveToPileEvent>(e =>
            {
                if (e.card.tile != null)
                {
                    e.card.tile.removeCard();
                    e.card.tile = null;
                }
                e.card.moveTo(e.pile);
            }));
        }

        private void raiseEvent(GameEvent e)
        {
            List<TriggeredAbility>[] preLists = players.Select(p => new List<TriggeredAbility>()).ToArray();
            List<TriggeredAbility>[] postLists = players.Select(p => new List<TriggeredAbility>()).ToArray();

            foreach (Card card in triggerableCards)
            {
                int controllerIndex = ord(card.controller);
                TriggeredAbility[] abilities = card.abilitiesTriggeredBy(e);

                foreach (TriggeredAbility ability in abilities)
                {
                    List<TriggeredAbility>[] addToMe;
                    switch (ability.timing)
                    {
                        case TriggeredAbility.Timing.Post:
                        {
                            addToMe = postLists;
                        } break;

                        case TriggeredAbility.Timing.Pre:
                        {
                            addToMe = preLists;
                        } break;

                        default: throw new Exception();
                    }
                    addToMe[controllerIndex].AddRange(abilities);
                }

            }

            bool remodify = false;

            foreach (Player p in players)
            {
                List<TriggeredAbility> triggered = preLists[ord(p)];
                if (triggered.Count > 1) throw new NotImplementedException();

                foreach (TriggeredAbility a in triggered)
                {
                    remodify = true;
                    var ts = getCastTargets(a, p.heroCard.tile);
                }
            }

            if (remodify)
            {
                foreach (Card card in cards)
                {
                    card.remodify(e);
                }
            }

            foreach (GameEventHandler handler in geFilters)
            {
                handler.handle(e);
            }

            foreach (Card card in cards)
            {
                card.remodify(e);
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

            Tile[] ts = new[]
            {
                map.tileAt(4, 4),
                map.tileAt(6, 6),
            };
            int ix = 0;

            foreach (Player p in players)
            {
                if (ix >= ts.Length) throw new Exception();

                ts[ix++].place(p.heroCard);
            }

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
            raiseEvent(new StartOfStepEvent(activePlayer, Steps.Untap));
            enforceRules();

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

            raiseEvent(new EndOfStepEvent(activePlayer, Steps.Untap));
            enforceRules();
        }

        private void untapStep()
        {
            activePlayer.resetMana();

            ManaOrbSelection selection;

            if (activePlayer == hero)
            {
                ManaPool pool = activePlayer.manaPool.clone();
                for (int i = 0; i < ManaSet.size; i++)
                {
                    if ((ManaColour)i == ManaColour.Colourless) continue;
                    pool.max[i]++;
                }
                activePlayer.stuntMana(pool);
                Controller.setPrompt("Gain mana nerd");
                ManaOrb v = (ManaOrb)waitForButtonOr<ManaOrb>(o => activePlayer.manaPool.max[(int)o.colour] != 6);
                activePlayer.unstuntMana();

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

                connection.sendAction(new MoveSelection(paths));
            }
            else
            {
                Controller.setPrompt("Opponent is moving");
                MoveSelection v = connection.receiveAction<MoveSelection>();
                paths = v.moves;
            }

            //todo raise MoveDeclared events lmfao
            priority();

            foreach (var v in paths)
            {
                Card c = v.Item1;
                Path p = v.Item2;
                raiseEvent(new MoveEvent(c, p));
            }

            Controller.clearArrows();

            enforceRules();
        }

        private void endStep()
        {
            activePlayerIndex = (activePlayerIndex + 1)%players.Count;
        }

        private void enforceRules()
        {
            List<Card> trashcan = new List<Card>();
            foreach (Card c in triggerableCards)
            {
                if (c.hasPT && c.toughness <= 0)
                {
                    trashcan.Add(c);
                }
            }

            foreach (Card c in trashcan)
            {
                raiseEvent(new MoveToPileEvent(c, c.owner.graveyard));
            }

            Controller.redraw();
        }

        private void priority()
        {
            int c = 0;
            while (true)
            {
                Player fuckboy = players[(activePlayerIndex + c)%players.Count];
                StackWrapper? w = priority(fuckboy);

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

                        //Controller.redraw();
                    }
                }
                enforceRules();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="f"></param>
        /// <param name="prompt"></param>
        /// <returns></returns>
        private Card getCard(Func<Card, bool> f, string prompt)
        {
            Controller.setPrompt(prompt, ButtonOption.Cancel);
            var v = waitForButtonOr<Card>(f);
            if (v is ShibbuttonStuff)
            {
                return null;
            }
            return (Card)v;
        }

        private Tile getTile(Func<Tile, bool> f, string prompt)
        {
            Controller.setPrompt(prompt, ButtonOption.OK);
            var v = waitForButtonOr<Tile>(f);
            if (v is ShibbuttonStuff)
            {
                return null;
            }
            return (Tile)v;
        }

        private StackWrapper? priority(Player p)
        {
            return cast(p, null);
        }

        private StackWrapper? cast(Player p, Card card = null, Ability ability = null, TargetMatrix[] targets = null, int[][] costs = null)
        {
            int lv = 0;
            int bt = 0;

            if (card != null) { lv = bt = 1; }
            if (ability != null) { lv = bt = 2; }

            StackWrapper? r = null;
            if (p == hero)
            {
                Tile from = p.heroCard.tile;
                while (lv >= bt && r == null)
                {
                    object stuff = null;
                    if (lv == 0)
                    {
                        stuff = card = getCard(c => c.controller == p, "Cast a card");
                    }
                    else if (lv == 1)
                    {
                        stuff = ability = chooseAbility(card);
                    }
                    else if (lv == 2)
                    {
                        stuff = targets = getCastTargets(ability, from);
                    }
                    else if (lv == 3)
                    {
                        CostPayStruct s = new CostPayStruct(waitForAnything);
                        stuff = costs = ability.cost.measure(p, s);
                    }
                    else if (lv == 4)
                    {
                        r = new StackWrapper(card, ability, targets, costs);
                    }

                    lv += stuff == null ? -1 : 1;
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
            ActivatedAbility r;
            ActivatedAbility[] v = c.usableHere;
            bool canCastSlow = activePlayer == hero && (stepHandler.step == Steps.Main1 || stepHandler.step == Steps.Main1);
            if (v.Length > 1) throw new Exception();
            if (v.Length == 0) return null;
            r = v[0];

            if (
                r.isInstant
                ||
                (stack.Count() == 0
                 &&
                 (stepHandler.step == Steps.Main1
                  ||
                  stepHandler.step == Steps.Main2
                     )
                 &&
                 activePlayer == hero
                    )
                )
            {
                return r;
            }
            else
            {
                return null;
            }
        }


        private TargetMatrix[] getCastTargets(Ability a, Tile castFrom)
        {
            TargetMatrix[] ms = new TargetMatrix[a.effects.Length];
            List<Tile> v = castFrom.withinDistance(a.castRange);

            Controller.highlight(v, Color.Green);
            Controller.setPrompt("target nigra", ButtonOption.Cancel);

            ChooseTargetToolbox box = new ChooseTargetToolbox(generateStuff(v));

            for (int i = 0; i < ms.Length; i++)
            {
                TargetMatrix tm = a.effects[i].ts.fillCast(box);
                if (tm == null)
                {
                    ms = null;
                    break;
                }
                ms[i] = tm;
            }
            Controller.clearHighlights();
            return ms;
        }
        

        private void resolve(StackWrapper wrapper)
        {
            Card card = wrapper.card;
            TargetMatrix[] ts = wrapper.matricies;
            Effect[] es = wrapper.ability.effects;



            if (ts.Length != es.Length) throw new Exception();

            List<GameEvent> events = new List<GameEvent>();

            for (int i = 0; i < ts.Length; i++)
            {
                Effect effect = es[i];
                TargetMatrix matrix = effect.ts.fillResolve(ts[i], card, this);

                events.AddRange(effect.doer.act(matrix.generateRows()));
            }

            foreach (GameEvent e in events)
            {
                raiseEvent(e);
            }

            if (card.cardType == CardType.Creature)
            {
                raiseEvent(new MoveToPileEvent(card, card.controller.field));
            }
            else
            {
                raiseEvent(new MoveToPileEvent(card, card.owner.graveyard));
            }

            enforceRules();
        }
        
        private ManualResetEventSlim callerBacker;
        private InputEventFilter filter;
        private Stuff waitedForStuff;
        

        private Func<Stuff> generateStuff(IEnumerable<Tile> allowedTiles)
        {
            Tile[] allowed = allowedTiles.ToArray();
            return () =>
            {
                while (true)
                {
                    Stuff v = waitForAnything();

                    if (v is Tile) { if (!allowed.Contains(v)) continue; }

                    return v;
                }
            };
        }

        public void clicked(Clickable c)
        {
            var stuff = c.getStuff();
            InputEvent e = new InputEvent(c, stuff);
            if (filter != null && filter.filter(e))
            {
                waitedForStuff = stuff;
                callerBacker.Set();
            }
        }

        private Stuff waitForButtonOr<T>(Func<T, bool> fn) where T : Stuff
        {
            InputEventFilter f = new InputEventFilter((clickable, o) => clickable is Shibbutton || (o is T && fn((T)o)));
            return waitFor(f);
        }

        private Stuff waitForAnything()
        {
            InputEventFilter f = new InputEventFilter((x, y) => true);
            return waitFor(f);
        }

        private Stuff waitFor(InputEventFilter f)
        {
            if (callerBacker != null || filter != null) throw new Exception();
            callerBacker = new ManualResetEventSlim();
            filter = f;
            callerBacker.Wait();
            var r = waitedForStuff;
            waitedForStuff = null;
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
