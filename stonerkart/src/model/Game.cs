
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public IEnumerable<Player> allPlayers => players;

        private List<Player> players;
        private List<Card> allCards;

        public IEnumerable<Card> ca => allCards;

        public Pile stack { get; } = new Pile(new Location(null, PileLocation.Stack));

        private List<TriggeredAbility> pendingTriggeredAbilities { get; }= new List<TriggeredAbility>();

        private int activePlayerIndex { get; set; }
        public Player activePlayer => players[activePlayerIndex];

        public StepHandler stepHandler;

        public GameController gameController;


        private GameEventHandlerBuckets baseHandler = new GameEventHandlerBuckets();
        private Stack<StackWrapper> wrapperStack { get; } = new Stack<StackWrapper>();

        private IEnumerable<Card> triggerableCards => allCards.Where(card => card.location.pile != PileLocation.Deck && !card.isDummy);
        private IEnumerable<Card> fieldCards => allCards.Where(card => card.location.pile == PileLocation.Field);

        private Random random;
        private GameConnection connection;
        private bool skipFirstDraw = true;

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
            map = new Map(11, 7, false, false);
            allCards = new List<Card>();
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

        public Card createToken(CardTemplate ct, Player owner)
        {
            Card r = createCard(ct, owner.field, owner);
            if (!r.isToken) throw new Exception();
            return r;
        }

        private Card createCard(CardTemplate ct, Pile pile, Player owner)
        {
            Card r = new Card(ct, owner);
            allCards.Add(r);
            r.moveTo(pile);
            return r;
        }

        private Card createDummy(Ability a, Pile pile)
        {
            Card r = a.createDummy();

            r.moveTo(pile);
            allCards.Add(r);
            return r;
        }

        public int ord(Card c)
        {
            return allCards.IndexOf(c);
        }

        public Card cardFromOrd(int i)
        {
            return allCards[i];
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
            baseHandler.add(new TypedGameEventHandler<PayManaEvent>(e =>
            {
                e.player.payMana(e.manaSet);
            }));

            baseHandler.add(new TypedGameEventHandler<GainBonusManaEvent>(e =>
            {
                e.player.gainBonusMana(e.colour);
            }));

            baseHandler.add(new TypedGameEventHandler<ShuffleDeckEvent>(e =>
            {
                e.player.deck.shuffle(random);
            }));
            
            baseHandler.add(new TypedGameEventHandler<DrawEvent>(e =>
            {
                for (int i = 0; i < e.cards; i++)
                {
                    e.player.deck.peek().moveTo(e.player.hand);
                }
            }));

            baseHandler.add(new TypedGameEventHandler<CastEvent>(e =>
            {
                wrapperStack.Push(e.wrapper);
                e.wrapper.stackCard.moveTo(stack);

                gameController.redraw();
            }));
        }

        public void startGame()
        {
            Thread t = new Thread(loopEx);
            t.Start();
        }

        private void initGame()
        {
            Deck d = DeckController.chooseDeck();

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
                map.tileAt(2, 2),
                map.tileAt(map.width - 3, map.height - 3),
            };
            int ix = 0;
            gameController.redraw();
            foreach (Player p in players)
            {
                if (ix >= ts.Length) throw new Exception();

                ts[ix++].place(p.heroCard);
            }

            int xi = random.Next();
            if (connection is DummyConnection) xi = ord(hero);
            Player flipwinner = players[xi%players.Count];
            ButtonOption bopt;
            if (flipwinner == hero)
            {
                bopt = waitForButton("Do you wish to go first?", ButtonOption.Yes, ButtonOption.No);
                connection.sendAction(new ChoiceSelection((int)bopt));
            }
            else
            {
                gameController.setPrompt("Oppenent won the flip and is choosing whether to go first.");
                ChoiceSelection v = connection.receiveAction<ChoiceSelection>();
                bopt = (ButtonOption)v.choices[0];
            }
            activePlayerIndex = (xi + (bopt == ButtonOption.Yes ? 0 : 1))%players.Count;

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
                        gameController.setPrompt("Opponent is redrawing.");
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
        }

        private void loopEx()
        {
            gameController.setPrompt("Game starting");
            initGame();
            gameController.redraw();


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
                case Steps.Replenish:
                {
                    untapStep();
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

            handleTransaction(new EndOfStepEvent(activePlayer, step));

            foreach (Player p in players)
            {
                p.clearBonusMana();
            }
        }

        private void untapStep()
        {
            activePlayer.resetMana();

            if (activePlayer.manaPool.max.orbs.Count() < 12)
            {
                ManaOrbSelection selection;
                if (activePlayer == hero)
                {
                    activePlayer.stuntMana();

                    gameController.setPrompt("Gain mana nerd");
                    ManaOrb v = (ManaOrb)waitForButtonOr<ManaOrb>(o => activePlayer.manaPool.currentMana(o.colour) != 6);

                    activePlayer.unstuntMana();

                    selection = new ManaOrbSelection(v.colour);
                    connection.sendAction(selection);
                }
                else
                {
                    gameController.setPrompt("Opponent is gaining mana");
                    selection = connection.receiveAction<ManaOrbSelection>();
                }

                activePlayer.gainMana(selection.orb);
            }
            priority();

            if (!skipFirstDraw)
            {
                handleTransaction(new DrawEvent(activePlayer, 1));
            }
            else
            {
                skipFirstDraw = false;
            }
        }

        private void mainStep()
        {
            priority();
        }

        private void moveStep()
        {
            List<Tuple<Card, Path>> paths;

            bool b = activePlayer == hero;
            gameController.setHeroActive(b);
            if (b)
            {
                paths = new List<Tuple<Card, Path>>();

                while (true)
                {
                    var from = getTile(tile => tile.card != null && tile.card.movement > 0 && tile.card.owner == activePlayer ,  "Move your cards nigra");
                    if (from == null) break;

                    Card card = from.card;
                    var tpl = paths.Find(tuple => tuple.Item1 == card); //find out if card already has plans to move

                    if (tpl != null)  //if it does cancel said plans
                    {
                        gameController.removeArrow(tpl.Item2);
                        paths.Remove(tpl);
                    }

                    var options = map.dijkstra(from).Where(path =>
                        path.length <= card.movement
                        &&
                        (
                            path.last.card == null ||
                            from.card.canAttack(path.last.card)
                        )
                        &&
                        !paths.Any(p => p.Item2.to == path.to)
                    ).ToList();

                    //IEnumerable<>

                    gameController.highlight(options.Select(n => new Tuple<Color, Tile>(Color.Green, n.last)));
                    var to = getTile(tile => tile == from || options.Select(p => p.last).Contains(tile), "Move to where?");
                    gameController.clearHighlights();
                    if (to == null) break;

                    if (to != from)
                    {
                        var path = options.First(p => p.last == to);
                        gameController.addArrow(path);
                        paths.Add(new Tuple<Card, Path>(card, path));
                    }
                }

                connection.sendAction(new MoveSelection(paths));
            }
            else
            {
                gameController.setPrompt("Opponent is moving");
                MoveSelection v = connection.receiveAction<MoveSelection>();
                paths = v.moves;
                foreach (var p in paths)
                {
                    gameController.addArrow(p.Item2);
                }
            }

            //todo raise MoveDeclared events lmfao
            priority();

            GameTransaction gt = new GameTransaction();

            foreach (var v in paths)
            {
                Card mover = v.Item1;
                Path path = v.Item2;
                Card defender = path.last.card;

                if (path.attacking && mover.canAttack(defender))
                {
                    gt.addEvent(new DamageEvent(mover, defender, mover.power));
                    if (defender.canRetaliate) gt.addEvent(new DamageEvent(defender, mover, defender.power));
                }
                
                gt.addEvent(new MoveEvent(mover, path));
                gt.addEvent(new FatigueEvent(mover, path.attacking ? mover.movement : path.length));
            }

            handleTransaction(gt);

            gameController.clearArrows();
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
                    pendAbilities(card.abilitiesTriggeredBy(e).Where(a => a.timing == TriggeredAbility.Timing.Pre));
                }
            }

            foreach (GameEvent e in gameEvents)
            {
                baseHandler.handle(e);

                foreach (Card card in allCards)
                {
                    card.handleEvent(e);
                }
            }

            foreach (GameEvent e in gameEvents)
            {
                foreach (Card card in triggerableCards)
                {
                    pendAbilities(card.abilitiesTriggeredBy(e).Where(a => a.timing == TriggeredAbility.Timing.Post));
                }
            }
        }

        private void pendAbilities(IEnumerable<TriggeredAbility> tas)
        {
            if (tas.Count() == 0) return;
            pendingTriggeredAbilities.AddRange(tas);
        }

        private void enforceRules()
        {
            foreach (Card c in allCards)
            {
                c.handleEvent(new ClearAurasEvent());
            }
            List<GameEvent> ae = new List<GameEvent>();
            foreach (Card c in allCards)
            {
                foreach (Aura a in c.auras)
                {
                    foreach (Card affected in allCards.Where(a.filter))
                    {
                        ae.Add(new ModifyEvent(affected, a.stat, a.modifer));
                    }
                }
            }

            foreach (Card c in allCards)
            {
                foreach (GameEvent e in ae)
                {
                    c.handleEvent(e);
                }
            }

            foreach (Card c in allCards)
            {
                c.updateState();
            }

            do
            {
                handlePendingTrigs();
            } while (pendingTriggeredAbilities.Count > 0);

            trashcanDeadCreatures(); //todo suspect

            gameController.redraw();
        }

        private void handlePendingTrigs()
        {
            if (pendingTriggeredAbilities.Count > 0)
            {
                List<TriggeredAbility>[] abilityArrays =
                    players.Select(p => new List<TriggeredAbility>()).ToArray();

                foreach (TriggeredAbility pending in pendingTriggeredAbilities)
                {
                    int ix = ord(pending.card.controller);
                    abilityArrays[ix].Add(pending);
                }

                List<StackWrapper> wrappers = new List<StackWrapper>();

                for (int i = 0; i < abilityArrays.Length; i++)
                {
                    Player p = playerFromOrd(i);
                    List<TriggeredAbility> abilityList = abilityArrays[i];
                    
                    wrappers.AddRange(handlePendingTrigs(p, abilityList));
                }

                pendingTriggeredAbilities.Clear();

                foreach (var w in wrappers)
                {
                    cast(w);
                }
            }
        }

        private List<StackWrapper> handlePendingTrigs(Player p, IEnumerable<TriggeredAbility> abilities)
        {
            List<StackWrapper> r = new List<StackWrapper>();
            TriggeredAbility[] orig = abilities.ToArray();

            Pile pl = new Pile();

            for (int i = 0; i < orig.Length; i++)
            {
                createDummy(orig[i], pl);
            }

            if (p == hero)
            {
                ManualResetEventSlim re = new ManualResetEventSlim();
                Clickable clkd = null;
                DraggablePanel dp = gameController.showPile(pl, false, c =>
                {
                    clkd = c;
                    re.Set();
                });

                List<ptas> triggd = new List<ptas>();

                while (triggd.Count < orig.Length)
                {
                    gameController.setPrompt("Select which ability to place on the stack next.");
                    re.Wait();
                    re.Reset();
                    Clickable cp = clkd;
                    if (!(cp is CardView)) continue;

                    CardView cv = (CardView)cp;
                    Card c = (Card)cv.getStuff();

                    ptas prevadd = triggd.FirstOrDefault(s => s.dummyCard == c);
                    if (prevadd != null)
                    {
                        triggd.Remove(prevadd);
                        cv.glowColour();
                        continue;
                    }

                    int i = Array.IndexOf(orig, c.dummiedAbility);
                    TriggeredAbility tab = orig[i];
                    StackWrapper v = tryCastDx(p, tab, c);
                    if (v == null) continue;

                    ptas ptas = new ptas(tab,i,c,v);
                    triggd.Add(ptas);
                    cv.glowColour(Color.Green);
                }

                dp.close();

                foreach (ptas pt in triggd)
                {
                    connection.sendAction(new CastSelection(pt.wrapper));
                    r.Add(pt.wrapper);
                }
            }
            else
            {
                gameController.setPrompt("Opponent is handling triggered abilities.");
                for (int i = 0; i < orig.Count(); i++)
                {
                    StackWrapper w = connection.receiveAction<CastSelection>().wrapper;
                    Card dummy = pl.First(c => c.dummiedAbility == w.ability);
                    
                    r.Add(new StackWrapper(dummy, w.ability, w.targetMatrices, w.costMatricies));
                }
            }
            return r;
        }
        #region hack
        private class ptas
        {
            public TriggeredAbility ability { get; }
            public int originalIndex { get; }
            public Card dummyCard { get; }
            public StackWrapper wrapper { get; }

            public ptas(TriggeredAbility ability, int originalIndex, Card dummyCard, StackWrapper wrapper)
            {
                this.ability = ability;
                this.originalIndex = originalIndex;
                this.dummyCard = dummyCard;
                this.wrapper = wrapper;
            }
        }
        #endregion 

        private void trashcanDeadCreatures()
        { 
            List<Card> trashcan = new List<Card>();
            foreach (Card c in fieldCards)
            {
                if (c.toughness <= 0 && c.cardType == CardType.Creature)
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
                StackWrapper w = tryCast(fuckboy);

                if (w != null)
                {
                    cast(w);
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
                        if (wrapper.castingCard != stack.peekTop() && wrapper.castingCard != stack.peekTop().dummyFor) throw new Exception();
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
        /// <param text="f"></param>
        /// <param text="prompt"></param>
        /// <returns></returns>
        private Card chooseCastCard(Player p)
        {
            gameController.setPrompt("Cast a card.", ButtonOption.Pass);
            while (true)
            {
                var v = waitForAnything();
                Card c = null;
                if (v is ShibbuttonStuff)
                {
                    return null;
                }
                else if (v is Card)
                {
                    c = (Card)v;
                }
                else if (v is Tile)
                {
                    Tile t = (Tile)v;
                    if (t.card != null) c = t.card;
                }

                if (c != null && c.controller == p) return c;

            }
        }

        /// <summary>
        /// Returns null when the OK button is pressed else it returns a tile when it is clicked
        /// </summary>
        /// <param name="f">Filters allowable tiles</param>
        /// <param name="prompt">The string to prompt to the user</param>
        /// <returns></returns>
        private Tile getTile(Func<Tile, bool> f, string prompt)
        {
            gameController.setPrompt(prompt, ButtonOption.OK);
            var v = waitForButtonOr<Tile>(f);
            if (v is ShibbuttonStuff)
            {
                return null;
            }
            return (Tile)v;
        }

        private void cast(StackWrapper w)
        {
            GameTransaction gt = new GameTransaction();
            gt.addEvents(w.ability.cost.resolve(makeHackStruct(w.ability), w.costMatricies));
            handleTransaction(gt);
            gt = new GameTransaction();
            gt.addEvent(new CastEvent(w));
            handleTransaction(gt);
        }

        private StackWrapper tryCast(Player p)
        { 
            StackWrapper r;

            bool b = p == hero;
            gameController.setHeroActive(b);
            if (b)
            {
                if ((stack.Any() ||
                     Settings.stopTurnSetting.getTurnStop(stepHandler.step, hero == activePlayer)))
                {
                    r = tryCastDx(hero);
                }
                else
                {
                    r = null;
                }
                connection.sendAction(new CastSelection(r));
            }
            else
            {
                gameController.setPrompt("Opponents turn to act.");
                r = connection.receiveAction<CastSelection>().wrapper;
            }

            if (r != null && !r.ability.isCastAbility)
            {
                r = new StackWrapper(r.ability.createDummy(), r.ability, r.targetMatrices, r.costMatricies);
            }
            return r;
        }


        private StackWrapper tryCastDx(Player p)
        {
            do
            {
                Card card;
                Ability ability;
                TargetMatrix[] costmxs;
                TargetMatrix[] targetmxs;

                if (!(stack.Any() ||
                        Settings.stopTurnSetting.getTurnStop(stepHandler.step, hero == activePlayer)))
                {
                    return null;    //auto pass
                }

                card = chooseCastCard(p);
                if (card == null) return null;

                ability = chooseAbility(card);
                if (ability == null) continue;

                targetmxs = chooseTargets(ability);
                if (targetmxs == null) continue;

                costmxs = ability.cost.fillCast(makeHackStruct(p));
                if (costmxs == null) continue;

                return new StackWrapper(card, ability, targetmxs, costmxs);
            } while (true);
        }

        private StackWrapper tryCastDx(Player p, TriggeredAbility ability, Card dummyCard)
        {
            TargetMatrix[] costmxs;
            TargetMatrix[] targetmxs;

            do
            {
                targetmxs = chooseTargets(ability);
                if (targetmxs == null) return null;

                costmxs = ability.cost.fillCast(makeHackStruct(p));
                if (costmxs == null) continue;

                return new StackWrapper(dummyCard, ability, targetmxs, costmxs);
            } while (true);
        }

        private ActivatedAbility chooseAbility(Card c)
        {
            ActivatedAbility r;
            ActivatedAbility[] activatableAbilities = c.usableHere;

            bool canCastSlow = stack.Count == 0 && activePlayer == hero && (stepHandler.step == Steps.Main1 || stepHandler.step == Steps.Main2);
            if (!canCastSlow)
            {
                activatableAbilities = activatableAbilities.Where(a => a.isInstant).ToArray();
            }

            if (activatableAbilities.Length > 1) throw new NotImplementedException();
            if (activatableAbilities.Length == 0) return null;

            r = activatableAbilities[0];

            return r;
        }

        private TargetMatrix[] chooseTargets(Ability a)
        {
            Card caster = a.isCastAbility ? a.card.controller.heroCard : a.card;
            List<Tile> v = caster.tile.withinDistance(a.castRange);

            HackStruct box = makeHackStruct(a);

            if (!a.possible(box)) return null;

            TargetMatrix[] ms = a.effects.fillCast(box);

            gameController.clearHighlights();
            return ms;
        }

        private void resolve(StackWrapper wrapper)
        {
            Card stackpopped = stack.peekTop();
            Card card = wrapper.castingCard;
            TargetMatrix[] ts = wrapper.targetMatrices;
            Ability ability = wrapper.ability;

            if (stackpopped.isDummy && stackpopped.dummyFor != card) throw new Exception();

            List<GameEvent> events = new List<GameEvent>();

            events.AddRange(ability.resolve(makeHackStruct(ability), ts));

            GameTransaction gt = new GameTransaction(events);


            if (stackpopped.isDummy)
            {
                stackpopped.pile.remove(stackpopped);
            }
            else if (stackpopped.cardType == CardType.Creature || stackpopped.cardType == CardType.Relic)
            {
                gt.addEvent(new MoveToPileEvent(stackpopped, stackpopped.controller.field, false));
            }
            else
            {
                gt.addEvent(new MoveToPileEvent(stackpopped, stackpopped.owner.graveyard, false));
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

        public Stuff waitForAnything()
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
            gameController.setPrompt(prompt, options);
            ShibbuttonStuff s = (ShibbuttonStuff)waitFor(new InputEventFilter((c, o) => c is Shibbutton));
            return s.option;
        }

        private HackStruct makeHackStruct(Player castingPlayer)
        {
            return new HackStruct(this, castingPlayer);
        }

        private HackStruct makeHackStruct(Ability resolvingAbility)
        {
            return new HackStruct(this, resolvingAbility);
        }
        

        private DraggablePanel showCards(IEnumerable<Card> cards, bool closeable)
        {
            return gameController.showCards(cards, closeable, clicked);
        }

        public IEnumerable<Card> selectCardFromCards(IEnumerable<Card> cards, bool cancelable = true, int cardCount = 1, Func<Card, bool> filter = null)
        {
            List<Card> rt = new List<Card>(cardCount);
            filter = filter == null ? (c) => true : filter;
            var ca = cards.ToArray();

            if (ca.Count(filter) == 0)
            {
                var vv = showCards(ca, false);
                var rr = waitForButton("No valid selections.", ButtonOption.Cancel);
                vv.close();
                return new Card[0];
            }

            gameController.setPrompt("Select card", cancelable ? new ButtonOption[]{ButtonOption.Cancel, } : new ButtonOption[]{ButtonOption.NOTHING});
            var v = showCards(ca, false);

            while (rt.Count < cardCount)
            {
                Stuff r = waitForButtonOr<Card>(c => ca.Contains(c) && filter(c));
                if (r is Card)
                {
                    Card card = (Card)r;
                    if (rt.Contains(card))
                    {
                        rt.Remove(card);
                    }
                    else
                    {
                        rt.Add(card);
                    }
                }
                if (r is ShibbuttonStuff)
                {
                    rt.Clear();
                    break;
                }
            }
            v.close();
            return rt;
        }

        public void sendChoices(int[] cs)
        {
            connection.sendAction(new ChoiceSelection(cs));
        }

        public int[] receiveChoices()
        {
            return connection.receiveAction<ChoiceSelection>().choices;
        }

        public void clearTargetHighlight()
        {
            gameController.clearHighlights();
        }

        public void setTargetHighlight(Card c)
        {
            StackWrapper w = wrapperStack.FirstOrDefault(sw => sw.stackCard == c);

            if (w == null) return;

            List<Tile> tiles = new List<Tile>();
            foreach (TargetMatrix tm in w.targetMatrices)
            {
                foreach (TargetColumn cl in tm.columns)
                {
                    if (cl.targets == null) continue;
                    tiles.AddRange(tilesFromTargetables(cl.targets));
                }
            }
            gameController.highlight(tiles, Color.OrangeRed);
        }

        private static IEnumerable<Tile> tilesFromTargetables(Targetable[] ts)
        {
            List<Tile> rt = new List<Tile>();
            foreach (var t in ts)
            {
                if (t is Tile)
                {
                    rt.Add((Tile)t);
                }
                if (t is Card)
                {
                    Card c = (Card)t;
                    if (c.tile != null) { rt.Add(c.tile); }
                }
                if (t is Player)
                {
                    rt.Add(((Player)t).heroCard.tile);
                }
            }
            return rt;
        }
    }


    class StackWrapper
    {
        public Card stackCard { get; }
        public Ability ability { get; }
        public TargetMatrix[] targetMatrices { get; }
        public TargetMatrix[] costMatricies { get; }

        public Card castingCard => stackCard.isDummy ? stackCard.dummyFor : stackCard;
        public bool isCastAbility => castingCard.isCastAbility(ability);


        public StackWrapper(Card stackCard, Ability ability, TargetMatrix[] targetMatrices, TargetMatrix[] costMatricies)
        {
            this.stackCard = stackCard;
            this.ability = ability;
            this.targetMatrices = targetMatrices;
            this.costMatricies = costMatricies;
        }
    }
    
    
    struct HackStruct
    {
        //game stuff
        public IEnumerable<Player> players { get; }
        public Player hero { get; }
        public Player castingPlayer { get; }
        public bool heroIsCasting => hero == castingPlayer;
        public Func<CardTemplate, Player, Card> createToken;

        public Tile castFrom => !resolveAbility.isCastAbility ? resolveAbility.card.tile : hero.heroCard.tile;
        public int castRange => resolveAbility.castRange;

        public Func<Card, int> ordC { get; }
        public Func<Player, int> ordP { get; }
        public Func<Tile, int> ordT { get; }
        public Func <int, Card> Cord { get; }
        public Func <int, Player> Pord { get; }
        public Func <int, Tile> Tord { get; }

        //resolve shit
        public Player resolveController => resolveCard.controller;
        public bool heroIsResolver => hero == resolveController;
        public Card resolveCard => resolveAbility.card;
        public Ability resolveAbility { get; }
        public IEnumerable<Card> cards { get; }

        public TargetMatrix previousTargets { get; set; }
        public TargetColumn previousColumn { get; set; }
        public IEnumerable<Tile> tilesInRange => castFrom.withinDistance(castRange);
        

        //network stuff
        public Action<int[]> sendChoices { get; }
        public Func<int[]> receiveChoices { get; }

        //ui stuff
        public Func<Stuff> getStuff { get; }
        private Action<string, ButtonOption[]> _setPrompt { get; }

        public Action clearHighlights { get; }
        public Action<IEnumerable<Tile>, Color> highlight { get; }

        private Func<IEnumerable<Card>, bool, int, Func<Card, bool>, IEnumerable<Card>> selectCardEx;


        public HackStruct(Game g) : this()
        {
            ordP = g.ord;
            ordC = g.ord;
            ordT = g.ord;
            Cord = g.cardFromOrd;
            Pord = g.playerFromOrd;
            Tord = g.tileFromOrd;
            cards = g.ca;
            sendChoices = g.sendChoices;
            receiveChoices = g.receiveChoices;
            selectCardEx = g.selectCardFromCards;
            getStuff = g.waitForAnything;
            hero = g.hero;
            _setPrompt = g.gameController.setPrompt;
            clearHighlights = () => g.gameController.clearHighlights(true);
            highlight = g.gameController.highlight;
            createToken = g.createToken;
            players = g.allPlayers;
        }

        public HackStruct(Game g, Player p) : this(g)
        {
            castingPlayer = p;
        }

        public HackStruct(Game g, Ability a) : this(g)
        {
            resolveAbility = a;
        }

        public void setPrompt(string s)
        {
            _setPrompt(s, new ButtonOption[] {});
        }

        public void setPrompt(string s, params ButtonOption[] os)
        {
            _setPrompt(s, os);
        }

        public T waitForStuff<T>(Func<T, bool> f) where T : Stuff
        {
            while (true)
            {
                Stuff s = getStuff();
                if (s is T)
                {
                    T t = (T)s;
                    if (f(t)) return t;
                }
            }
        }

        public Card selectCardSynchronized(IEnumerable<Card> cs, Player chooser, Func<Card, bool> filter)
        {
            var v = selectCardsSynchronized(cs, chooser, 1, filter).ToArray();
            if (v.Length == 0) return null;
            return v[0];
        }

        public IEnumerable<Card> selectCardsSynchronized(IEnumerable<Card> cs, Player chooser, int cardCount, Func<Card, bool> filter)
        {
            IEnumerable<Card> c;
            if (hero == chooser)
            {
                setPrompt("Choose a card.");
                c = selectCardEx(cs, false, cardCount, filter);
                int[] ss = c.Select(ordC).ToArray();
                sendChoices(ss);
            }
            else
            {
                setPrompt("Opponent is choosing a card.");
                int[] v = receiveChoices();
                c = v.Select(Cord);
            }
            return c;
        }
    }
    
    class StepHandler : Observable<Steps>
    {
        public Steps step { get; private set; }

        public void nextStep()
        {
            if (step == Steps.End) step = Steps.Replenish;
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

        public void addEvents(IEnumerable<GameEvent> es)
        {
            events.AddRange(es);
        }
    }

    enum Steps
    {
        Replenish,
        Main1,
        Move,
        Main2,
        End
    }
}
