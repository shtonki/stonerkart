
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

        public Pile stack { get; } = new Pile(new Location(null, PileLocation.Stack));

        private List<PendingAbilityStruct> pendingTriggeredAbilities = new List<PendingAbilityStruct>();

        private int activePlayerIndex { get; set; }
        public Player activePlayer => players[activePlayerIndex];

        public StepHandler stepHandler;

        private List<GameEventHandler> geFilters = new List<GameEventHandler>();
        private Stack<StackWrapper> wrapperStack = new Stack<StackWrapper>();

        private IEnumerable<Card> triggerableCards => cards.Where(card => card.location.pile != PileLocation.Deck && !card.isDummy);
        private IEnumerable<Card> fieldCards => cards.Where(card => card.location.pile == PileLocation.Field);

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

                players.Add(p);
                if (i == ngs.heroIndex) hero = p;
                else
                {
                    if (villain != null) throw new Exception();
                    villain = p;
                }
            }

            stepHandler = new StepHandler();

            setupHandlers();
        }

        public Card createCard(CardTemplate ct, Pile pile, Player owner = null, bool dummy = false)
        {
            Card r = new Card(ct, owner, dummy);
            cards.Add(r);
            r.moveTo(pile);
            return r;
        }

        private Card createDummy(Card from, Pile pile)
        {
            return createCard(from.template, pile, from.controller, true);
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
                for (int i = 0; i < e.cards; i++)
                {
                    e.player.deck.peek().moveTo(e.player.hand);
                }
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
                    mover.movement.modify(-path.length, ModifiableSchmoo.intAdd, ModifiableSchmoo.never);
                }
                else
                {
                    if (path.penultimate.card != null && path.penultimate.card != mover) throw new Exception();

                    mover.moveTo(path.penultimate);
                    mover.exhaust();
                }
            }));

            geFilters.Add(new GameEventHandler<DamageEvent>(e =>
            {
                e.target.toughness.modify(-e.amount, ModifiableSchmoo.intAdd, ModifiableSchmoo.never);
            }));

            geFilters.Add(new GameEventHandler<MoveToPileEvent>(e =>
            {
                if (e.nullTile && e.card.tile != null)
                {
                    e.card.tile.removeCard();
                    e.card.tile = null;
                }
                e.card.moveTo(e.to);
            }));
        }

        public void startGame()
        {
            Thread t = new Thread(loopEx);
            t.Start();
        }

        private void initGame()
        {
            Deck d = Controller.chooseDeck();

            Deck[] decks = connection.deckify(d, ord(hero));

            for (int i = 0; i < players.Count; i++)
            {
                Player p = players[i];
                Deck deck = decks[i];

                Card heroCard = createCard(deck.hero, p.field, p);
                p.setHeroCard(heroCard);

                foreach (var ct in deck.templates)
                {
                    createCard(ct, p.deck, p);
                }
                p.deck.shuffle(random);
            }


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

            int xi = random.Next();
            activePlayerIndex = xi%players.Count;

            int[] draws = {5, 5, 4, 3, 2, 1};

            for (int i = 0; i < players.Count; i++)
            {
                for (int j = 0; j < draws.Length; j++)
                {
                    Player p = players[(i + activePlayerIndex)%players.Count];
                    int cards = draws[j];
                    handleTransaction(new DrawEvent(p, cards));

                    if (j == draws.Length - 1) break;

                    ChoiceSelection cs;

                    if (p == hero)
                    {
                        ButtonOption b = waitForButton(String.Format("Redraw to {0}?", draws[j+1]), ButtonOption.Yes, ButtonOption.No);
                        cs = new ChoiceSelection((int)b);
                        connection.sendAction(cs);
                    }
                    else
                    {
                        cs = connection.receiveAction<ChoiceSelection>();
                    }

                    ButtonOption bo = cs.choices.Length > 0 ? (ButtonOption)cs.choices[0] : ButtonOption.No;

                    if (bo == ButtonOption.Yes)
                    {
                        while (p.hand.Count > 0)
                        {
                            Card crd = p.hand.peek();
                            crd.moveTo(p.deck);
                        }
                        p.deck.shuffle(random);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            foreach (Player p in players)
            {
                p.deck.shuffle(random);
            }

            Controller.redraw();
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
            handleTransaction(new StartOfStepEvent(activePlayer, step));
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

            handleTransaction(new EndOfStepEvent(activePlayer, Steps.Untap));
        }

        private void untapStep()
        {
            foreach (Player p in players)
            {
                p.setActive(p == activePlayer);
            }

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

            priority();
        }

        private void drawStep()
        {
            handleTransaction(new DrawEvent(activePlayer, 1));

            priority();
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

            GameTransaction gt = new GameTransaction();

            foreach (var v in paths)
            {
                Card card = v.Item1;
                Path path = v.Item2;

                if (path.to.card != null)
                {
                    Card mover = card;
                    Card defender = path.to.card;

                    gt.addEvent(new DamageEvent(mover, defender, mover.power));
                    if (!defender.isHeroic) gt.addEvent(new DamageEvent(defender, mover, defender.power));
                }
                
                gt.addEvent(new MoveEvent(card, path));
            }

            handleTransaction(gt);

            Controller.clearArrows();
        }

        private void endStep()
        {
            priority();

            activePlayerIndex = (activePlayerIndex + 1)%players.Count;
        }

        private void handleTransaction(GameEvent e)
        {
            handleTransaction(new GameTransaction(e));
        }

        private void handleTransaction(GameTransaction t)
        {
            var gameEvents = t.events;

            foreach (GameEvent e in gameEvents)
            {
                foreach (Card card in triggerableCards)
                {
                    var abilities = card.abilitiesTriggeredBy(e);
                    var card1 = card;
                    pendAbilities(abilities.Where(a => a.timing == TriggeredAbility.Timing.Pre).Select(a => new PendingAbilityStruct(a, card1)));
                }
            }


            foreach (GameEvent e in gameEvents)
            {
                foreach (GameEventHandler handler in geFilters)
                {
                    handler.handle(e);
                }

                foreach (Card card in cards)
                {
                    card.remodify(e);
                }
            }

            foreach (GameEvent e in gameEvents)
            {
                foreach (Card card in triggerableCards)
                {
                    var abilities = card.abilitiesTriggeredBy(e);
                    var card1 = card;
                    pendAbilities(abilities.Where(a => a.timing == TriggeredAbility.Timing.Post).Select(a => new PendingAbilityStruct(a, card1)));
                }
            }
        }

        private void pendAbilities(IEnumerable<PendingAbilityStruct> tas)
        {
            if (tas.Count() == 0) return;
            pendingTriggeredAbilities.AddRange(tas);
        }

        private void enforceRules()
        {
            do
            {
                handlePendingTrigs();
                trashcanDeadCreatures();
            } while (pendingTriggeredAbilities.Count > 0);
            Controller.redraw();
        }

        private void handlePendingTrigs()
        {
            if (pendingTriggeredAbilities.Count > 0)
            {
                List<PendingAbilityStruct>[] abilityArrays =
                    players.Select(p => new List<PendingAbilityStruct>()).ToArray();

                foreach (PendingAbilityStruct pending in pendingTriggeredAbilities)
                {
                    int ix = ord(pending.card.controller);
                    abilityArrays[ix].Add(pending);
                }

                for (int i = 0; i < abilityArrays.Length; i++)
                {
                    Player p = playerFromOrd(i);
                    List<PendingAbilityStruct> abilityList = abilityArrays[i];

                    if (abilityList.Count > 1) throw new NotImplementedException();

                    foreach (PendingAbilityStruct str in abilityList)
                    {
                        var v = cast(p, false, createDummy(str.card, p.displaced), str.ability);
                        if (!v.HasValue) throw new Exception();
                        playerCasts(p, v.Value);
                    }
                }
            }
            pendingTriggeredAbilities.Clear();
        }

        private void trashcanDeadCreatures()
        { 
            List<Card> trashcan = new List<Card>();
            foreach (Card c in fieldCards)
            {
                if (c.toughness <= 0)
                {
                    trashcan.Add(c);
                }
            }

            GameTransaction gt = new GameTransaction();

            foreach (Card c in trashcan)
            {
                gt.addEvent(new MoveToPileEvent(c, c.owner.graveyard));
            }

            handleTransaction(gt);
        }

        private void priority()
        {
            int c = 0;
            while (true)
            {
                enforceRules();
                Player fuckboy = players[(activePlayerIndex + c)%players.Count];
                StackWrapper? w = priority(fuckboy);

                if (w.HasValue)
                {
                    playerCasts(activePlayer, w.Value);
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

        private void playerCasts(Player p, StackWrapper w)
        {
            GameTransaction gt = new GameTransaction();
            gt.addEvent(new PayCostsEvent(p, w.ability, w.costs));
            gt.addEvent(new CastEvent(w));
            handleTransaction(gt);
        }

        private StackWrapper? priority(Player p)
        {
            return cast(p, true);
        }

        private StackWrapper? cast(Player p, bool cancellable, Card card = null, Ability ability = null, TargetMatrix[] targets = null, int[][] costs = null)
        {
            int lv = 0;
            int bt = 0;

            if (card != null) { lv = bt = 1; }
            if (ability != null) { lv = bt = 2; }

            StackWrapper? r = null;
            if (p == hero)
            {
                if (cancellable && !Settings.stopTurnSetting.getTurnStop(stepHandler.step, hero == activePlayer)) return null;
                Tile from = p.heroCard.tile;
                while (lv >= bt && r == null)
                {
                    object stuff = null;
                    if (lv == 0)
                    {
                        stuff = card = getCard(c => c.controller == p, "Cast a card");
                        if (cancellable && stuff == null) return null;
                    }
                    else if (lv == 1)
                    {
                        Ability ab = chooseAbility(card);
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
                        stuff = r = new StackWrapper(card, ability, targets, costs);
                    }
                    lv = stuff == null ? bt : lv+1;
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
            ActivatedAbility[] activatableAbilities = c.usableHere;

            bool canCastSlow = stack.Count == 0 && activePlayer == hero && (stepHandler.step == Steps.Main1 || stepHandler.step == Steps.Main1);
            if (!canCastSlow)
            {
                activatableAbilities = activatableAbilities.Where(a => a.isInstant).ToArray();
            }

            if (activatableAbilities.Length > 1) throw new NotImplementedException();
            if (activatableAbilities.Length == 0) return null;

            r = activatableAbilities[0];

            return r;
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

            GameTransaction gt = new GameTransaction(events);

            if (card.isDummy)
            {
                card.pile.remove(card);
            }
            else if (card.cardType == CardType.Creature)
            {
                gt.addEvent(new MoveToPileEvent(card, card.controller.field, false));
            }
            else
            {
                gt.addEvent(new MoveToPileEvent(card, card.owner.graveyard, false));
            }

            handleTransaction(gt);
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

        private ButtonOption waitForButton(string prompt, params ButtonOption[] options)
        {
            Controller.setPrompt(prompt, options);
            ShibbuttonStuff s = (ShibbuttonStuff)waitFor(new InputEventFilter((c, o) => c is Shibbutton));
            return s.option;
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

    class GameTransaction
    {
        public List<GameEvent> events { get; private set; }

        public GameTransaction()
        {
            events = new List<GameEvent>();
        }

        public GameTransaction(IEnumerable<GameEvent> ges) : this(ges.ToArray())
        {
        }

        public GameTransaction(params GameEvent[] ges)
        {
            events = new List<GameEvent>(ges);
        }

        public void addEvent(GameEvent e)
        {
            events.Add(e);
        }
    }

    internal struct PendingAbilityStruct
    {
        public TriggeredAbility ability { get; }
        public Card card { get; }

        public PendingAbilityStruct(TriggeredAbility ability, Card card)
        {
            this.ability = ability;
            this.card = card;
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
