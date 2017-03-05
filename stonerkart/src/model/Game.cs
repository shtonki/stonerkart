
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

        private List<Player> players;
        private List<Card> cards;

        public Pile stack { get; } = new Pile(new Location(null, PileLocation.Stack));

        private List<TriggeredAbility> pendingTriggeredAbilities = new List<TriggeredAbility>();

        private int activePlayerIndex { get; set; }
        public Player activePlayer => players[activePlayerIndex];

        public StepHandler stepHandler;

        private GameEventHandlerBuckets baseHandler = new GameEventHandlerBuckets();
        private Stack<StackWrapper> wrapperStack { get; } = new Stack<StackWrapper>();

        private IEnumerable<Card> triggerableCards => cards.Where(card => card.location.pile != PileLocation.Deck && !card.isDummy);
        private IEnumerable<Card> fieldCards => cards.Where(card => card.location.pile == PileLocation.Field);

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
            map = new Map(16, 9, false, false);
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

        public Card createCard(CardTemplate ct, Pile pile, Player owner = null)
        {
            Card r = new Card(ct, owner);
            cards.Add(r);
            r.moveTo(pile);
            return r;
        }

        private Card createDummy(Card from, Pile pile)
        {
            Card r = from.clone();
            r.moveTo(pile);
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

                Controller.redraw();
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
                map.tileAt(4, 3),
                map.tileAt(map.width - 6, map.height - 4),
            };
            int ix = 0;
            Controller.redraw();
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
                Controller.setPrompt("Oppenent won the flip and is choosing whether to go first.");
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
                        Controller.setPrompt("Opponent is redrawing.");
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
            Controller.setPrompt("Game starting");
            initGame();
            Controller.redraw();


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

            handleTransaction(new EndOfStepEvent(activePlayer, step));

            foreach (Player p in players)
            {
                p.clearBonusMana();
            }
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
                activePlayer.stuntMana();

                Controller.setPrompt("Gain mana nerd");
                ManaOrb v = (ManaOrb)waitForButtonOr<ManaOrb>(o => activePlayer.manaPool.currentMana(o.colour) != 6);

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
            if (!skipFirstDraw)
            {
                handleTransaction(new DrawEvent(activePlayer, 1));
            }
            else
            {
                skipFirstDraw = false;
            }

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
                    var tpl = paths.Find(tuple => tuple.Item1 == card); //find out if card already has plans to move

                    if (tpl != null)  //if it does cancel said plans
                    {
                        Controller.removeArrow(tpl.Item2);
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

                    Controller.highlight(options.Select(n => new Tuple<Color, Tile>(Color.Green, n.last)));
                    var to = getTile(tile => tile == from || options.Select(p => p.last).Contains(tile), "Move to where?");
                    Controller.clearHighlights();
                    if (to == null) break;

                    if (to != from)
                    {
                        var path = options.First(p => p.last == to);
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
                foreach (var p in paths)
                {
                    Controller.addArrow(p.Item2);
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
                    pendAbilities(card.abilitiesTriggeredBy(e).Where(a => a.timing == TriggeredAbility.Timing.Pre));
                }
            }

            foreach (GameEvent e in gameEvents)
            {
                baseHandler.handle(e);

                foreach (Card card in cards)
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
            foreach (Card c in cards)
            {
                c.updateState();
            }

            do
            {
                handlePendingTrigs();
            } while (pendingTriggeredAbilities.Count > 0);

            trashcanDeadCreatures(); //todo suspect

            Controller.redraw();
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

                for (int i = 0; i < abilityArrays.Length; i++)
                {
                    Player p = playerFromOrd(i);
                    List<TriggeredAbility> abilityList = abilityArrays[i];
                    
                    handlePendingTrigs(p, abilityList);
                }
                pendingTriggeredAbilities.Clear();
            }
        }

        private void handlePendingTrigs(Player p, IEnumerable<TriggeredAbility> abilityList)
        {
            if (p == hero)
            {
                List<TriggeredAbility> pending = abilityList.ToList();

                foreach (TriggeredAbility str in pending)
                {
                    Card c = createDummy(str.card, str.card.owner.displaced);
                    StackWrapper v = tryCastDx(p, str, c);
                    if (v == null) throw new Exception();

                    playerCasts(p, v);
                }
            }
            else
            {
                //throw new NotImplementedException("if you weren't expecting too see this you might be in some trouble son");
            }
        }

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
                    playerCasts(fuckboy, w);
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
        private Card getCard(Func<Card, bool> f, string prompt)
        {
            Controller.setPrompt(prompt, ButtonOption.Cancel);
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

                if (c != null && f(c)) return c;

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
            Controller.setPrompt(prompt, ButtonOption.OK);
            var v = waitForButtonOr<Tile>(f);
            if (v is ShibbuttonStuff)
            {
                return null;
            }
            return (Tile)v;
        }

        /// <summary>
        /// Wrapper function which one calls when one wants a Player to put a card on the stack using the information in a  StackWrapper's.
        /// </summary>
        /// <param text="p">The Player who is to cast the spell/ability.</param>
        /// <param text="w">The StackWrapper containing the relevant information.</param>
        private void playerCasts(Player p, StackWrapper w)
        {
            GameTransaction gt = new GameTransaction();
            gt.addEvents(w.ability.cost.resolve(makeHackStruct(w.castingCard), w.costMatricies));
            handleTransaction(gt);
            gt = new GameTransaction();
            gt.addEvent(new CastEvent(w));
            handleTransaction(gt);
        }

        private StackWrapper tryCast(Player p)
        { 
            StackWrapper r;
            if (p == hero)
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
                Controller.setPrompt("Opponents turn to act.");
                r = connection.receiveAction<CastSelection>().wrapper;
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

                card = getCard(c => c.controller == p, "Cast a card");
                if (card == null) return null;

                ability = chooseAbility(card);
                if (ability == null) continue;

                targetmxs = getCastTargets(ability);
                if (targetmxs == null) continue;

                costmxs = ability.cost.fillCast(makeHackStruct(waitForAnything));
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
                targetmxs = getCastTargets(ability);
                if (targetmxs == null) return null;

                costmxs = ability.cost.fillCast(makeHackStruct(waitForAnything));
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

        private TargetMatrix[] getCastTargets(Ability a)
        {
            Card caster = a.isCastAbility ? a.card.controller.heroCard : a.card;
            List<Tile> v = caster.tile.withinDistance(a.castRange);

            Controller.highlight(v, Color.Green);
            Controller.setPrompt("target nigra", ButtonOption.Cancel);

            HackStruct box = makeHackStruct(generateStuff(v));
            TargetMatrix[] ms = a.effects.fillCast(box);

            Controller.clearHighlights();
            return ms;
        }

        private void resolve(StackWrapper wrapper)
        {
            Card stackpopped = stack.peekTop();
            Card card = wrapper.castingCard;
            TargetMatrix[] ts = wrapper.targetMatrices;
            Foo foo = wrapper.ability.effects;

            if (stackpopped.isDummy && stackpopped.dummyFor != card) throw new Exception();

            List<GameEvent> events = new List<GameEvent>();

            events.AddRange(foo.resolve(makeHackStruct(card), ts));

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

        private HackStruct makeHackStruct()
        {
            return new HackStruct(selectCardFromCards, hero, activePlayer, sendChoices, receiveChoices, waitForAnything, ord, ord,
                ord, cardFromOrd, playerFromOrd, tileFromOrd, cards, null, null);
        }

        private HackStruct makeHackStruct(Func<Stuff> f)
        {
            return new HackStruct(selectCardFromCards, hero, activePlayer, sendChoices, receiveChoices, f, ord, ord, ord,
                cardFromOrd, playerFromOrd, tileFromOrd, cards, null, null);
        }

        private HackStruct makeHackStruct(Card c)
        {
            return new HackStruct(selectCardFromCards, hero, activePlayer, sendChoices, receiveChoices, waitForAnything, ord, ord,
                ord, cardFromOrd, playerFromOrd, tileFromOrd, cards, c, null);
        }
        

        private DraggablePanel showCards(IEnumerable<Card> cards, bool closeable)
        {
            return Controller.showCards(cards, closeable, clicked);
        }

        private Card selectCardFromCards(IEnumerable<Card> cards, bool cancelable, int cardCount)
        {
            if (cardCount != 1) throw new NotImplementedException();
            Controller.setPrompt("Select card", cancelable ? new ButtonOption[]{ButtonOption.Cancel, } : new ButtonOption[]{ButtonOption.NOTHING});
            var v = showCards(cards, false);
            Stuff r = waitForButtonOr<Card>(c => cards.Contains((c)));
            v.close();
            if (r is Card)
            {
                return (Card)r;
            }
            if (r is ShibbuttonStuff)
            {
                return null;
            }
            throw new Exception();
        }

        private void sendChoices(int[] cs)
        {
            connection.sendAction(new ChoiceSelection(cs));
        }

        private int[] receiveChoices()
        {
            return connection.receiveAction<ChoiceSelection>().choices;
        }

        public void clearTargetHighlight()
        {
            Controller.clearHighlights();
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
                    tiles.AddRange(tilesFromTargetables(cl.targets));
                }
            }
            Controller.highlight(tiles, Color.OrangeRed);
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
        public Player hero { get; }
        public Player activePlayer { get; }
        public bool heroIsActive => hero == activePlayer;


        public Func<Card, int> ordC { get; }
        public Func<Player, int> ordP { get; }
        public Func<Tile, int> ordT { get; }
        public Func <int, Card> Cord { get; }
        public Func <int, Player> Pord { get; }
        public Func <int, Tile> Tord { get; }


        //resolve shit
        public Player resolveController => resolveCard.controller;
        public bool heroIsResolver => hero == resolveController;
        public Card resolveCard { get; }
        public IEnumerable<Card> cards { get; }
        public TargetMatrix previousTargets { get; set; }

        //network stuff
        public Action<int[]> sendChoices { get; }
        public Func<int[]> receiveChoices { get; }

        //ui stuff
        public Func<Stuff> getStuff { get; }
        


        private Func<IEnumerable<Card>, bool, int, Card> selectCardEx;
        
        public HackStruct(Func<IEnumerable<Card>, bool, int, Card> selectCardEx, Player hero, Player activePlayer, Action<int[]> sendChoices, Func<int[]> receiveChoices, Func<Stuff> getStuff, Func<Card, int> ordC, Func<Player, int> ordP, Func<Tile, int> ordT, Func<int, Card> cord, Func<int, Player> pord, Func<int, Tile> tord, IEnumerable<Card> cards, Card resolveCard, TargetMatrix previousTargets)
        {
            this.selectCardEx = selectCardEx;
            this.hero = hero;
            this.activePlayer = activePlayer;
            this.sendChoices = sendChoices;
            this.receiveChoices = receiveChoices;
            this.getStuff = getStuff;
            this.ordC = ordC;
            this.ordP = ordP;
            this.ordT = ordT;
            Cord = cord;
            Pord = pord;
            Tord = tord;
            this.cards = cards;
            this.resolveCard = resolveCard;
            this.previousTargets = previousTargets;
        }


        public Card selectCardSynchronized(IEnumerable<Card> cs)
        {
            Card c;
            if (heroIsResolver)
            {
                Controller.setPrompt("Choose a card.");
                c = selectCardEx(cs, false, 1);
                int s = ordC(c);
                sendChoices(new int[] {s});
            }
            else
            {
                Controller.setPrompt("Opponent is choosing a card.");
                int[] v = receiveChoices();
                Debug.Assert(v.Length == 1);
                c = Cord(v[0]);
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
        Draw,
        Main1,
        Move,
        Main2,
        End
    }
}
