﻿
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
        private List<Card> cards;

        public IEnumerable<Card> ca => cards;

        public Pile stack { get; } = new Pile(new Location(null, PileLocation.Stack));

        private List<TriggerGlueHack> pendingTriggeredAbilities { get; }= new List<TriggerGlueHack>();

        private int activePlayerIndex { get; set; }
        public Player activePlayer => players[activePlayerIndex];

        public StepHandler stepHandler;

        public GameController gameController;

        public int gameid { get; }

        private GameEventHandlerBuckets baseHandler = new GameEventHandlerBuckets();
        private Stack<StackWrapper> wrapperStack { get; } = new Stack<StackWrapper>();

        private IEnumerable<Card> triggerableCards => cards.Where(card => card.location.pile != PileLocation.Deck && !card.isDummy);
        private IEnumerable<Card> fieldCards => cards.Where(card => card.location.pile == PileLocation.Field);

        private Random random;
        private GameConnection connection;
        private bool skipFirstDraw = true;

        public Game(NewGameStruct ngs, bool local)
        {
            gameid = ngs.gameid;

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
            cards = new List<Card>();
            players = new List<Player>();

            for (int i = 0; i < ngs.playerNames.Length; i++)
            {
                Player p = new Player(this, ngs.playerNames[i]);

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
            cards.Add(r);
            r.moveTo(pile);
            return r;
        }

        private Card createDummy(Ability a, Pile pile)
        {
            Card r = a.createDummy();

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
            return map.path(c, t);
        }

        private void setupHandlers()
        {
            baseHandler.add(new TypedGameEventHandler<MoveToPileEvent>(e =>
            {
                if (e.card == hero.heroCard)
                {
                    forfeit(GameEndStateReason.Flop);
                }

                if (e.card.location.pile == PileLocation.Stack)
                {
                    Stack<StackWrapper> t = new Stack<StackWrapper>();
                    while (wrapperStack.Count > 0) //todo this is p ugly coachella
                    {
                        var v = wrapperStack.Pop();
                        if (v.stackCard == e.card) break;
                        t.Push(v);
                    }
                    while (t.Count > 0)
                    {
                        wrapperStack.Push(t.Pop());
                    }
                }
            }));

            baseHandler.add(new TypedGameEventHandler<PayManaEvent>(e =>
            {
                e.player.payMana(e.manaSet);
            }));

            baseHandler.add(new TypedGameEventHandler<GainBonusManaEvent>(e =>
            {
                e.player.gainBonusMana(e.orb.colour);
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

            if (!skipFirstDraw)
            {
                handleTransaction(new DrawEvent(activePlayer, 1));
            }
            else
            {
                skipFirstDraw = false;
            }

            if (activePlayer.manaPool.max.orbs.Count() < 12)
            {
                var mc = selectManaColour(activePlayer, o => activePlayer.manaPool.currentMana(o.colour) != 6);
                activePlayer.gainMana(mc);
            }

            priority();
        }

        private void mainStep()
        {
            priority();
        }

        private static Color[] clrs = new[]
        {
            Color.ForestGreen, 
            Color.Firebrick, 
            Color.Fuchsia, 
            Color.Ivory, 
            Color.DimGray, 
            Color.DarkCyan, 
            Color.PaleVioletRed, 
        };

        private void moveStep()
        {
            List<Tuple<Card, Path>> paths;

            bool b = activePlayer == hero;
            gameController.setHeroActive(b);
            var movers = activePlayer.field.Where(c => c.canMove);

            if (b)
            {
                var unpathed = movers.ToList();
                var occupado = activePlayer.field.Where(c => !c.canMove).Select(c => c.tile).ToList();

                while (true)
                {
                    gameController.highlight(unpathed.Select(c => c.tile), Color.DodgerBlue);
                    //gameController.highlight(pathed.Select(c => c.tile), Color.ForestGreen);

                    var from = getTile(
                        tile => tile.card != null && tile.card.canMove && tile.card.owner == activePlayer,
                        "Move your cards nigra", unpathed.Count == 0 ? ButtonOption.Pass : ButtonOption.NOTHING);
                    if (from == null) break;
                    Card mover = from.card;
                    if (mover.combatPath != null)
                    {
                        gameController.removeArrow(mover.combatPath);
                        occupado.Remove(mover.combatPath.last);
                    }
                    Path pth = new Path(from);
                    pth.colorHack = clrs[ord(mover)%clrs.Length];
                    gameController.addArrow(pth);

                    while (true)
                    {

                        var options = map.pathCosts(mover, from).Where(path =>
                            path.length <= mover.movement - pth.length //card can reach the tile
                            &&
                            (
                                path.to.card == null || // tile is empty
                                mover.canAttack(path.to.card) || //card can attack the card in the target tile
                                (path.to.card.controller == mover.controller && !occupado.Contains(path.to))
                                //we want to move to a tile occupied by a friendly creature that's moving somewhere else
                                )
                            &&
                            !occupado.Any(p => p == path.last) //there isn't a card in the tile we want to move to
                            ).ToList();
                        if (options.Count == 1) break;
                        var v = map.tyles.Where(t => t.card?.controller == mover.controller).ToArray();
                        gameController.highlight(options.Select(p => p.to), Color.Green);
                        var to = getTile(tile => options.Select(p => p.to).Contains(tile), "Move to where?");
                        gameController.clearHighlights();
                        if (to == null) continue;
                        if (to == from) break;

                        var cp = options.First(p => p.to == to);

                        gameController.removeArrow(pth);
                        pth.concat(cp);
                        gameController.addArrow(pth);
                        from = pth.to;

                        if (pth.length == mover.movement) break;
                    }

                    if (mover.combatPath == null)
                    {
                        unpathed.Remove(mover);
                    }

                    occupado.Add(pth.last);
                    mover.combatPath = pth;
                }
                paths = movers.Select(c => new Tuple<Card, Path>(c, c.combatPath)).ToList();
                connection.sendAction(new MoveSelection(paths));
            }
            else
            {
                gameController.setPrompt("Opponent is moving");
                MoveSelection v = connection.receiveAction<MoveSelection>();
                paths = v.moves;
                foreach (var p in paths)
                {
                    p.Item2.colorHack = clrs[ord(p.Item1) % clrs.Length];
                    gameController.addArrow(p.Item2);
                }
            }

            //todo raise MoveDeclared events lmfao
            priority();

            #region ambush shenans
            GameTransaction gt = new GameTransaction();
            var pathsx = paths.Where(p => p.Item1.hasAbility(KeywordAbility.Ambush) && p.Item2.attacking);
            foreach (var v in pathsx)
            {
                Card mover = v.Item1;
                Path path = v.Item2;
                Card defender = path.to.card;

                if (mover.canAttack(defender))
                {
                    gt.addEvent(new DamageEvent(mover, defender, mover.combatDamageTo(defender)));
                }

                gt.addEvent(new MoveEvent(mover, path));
                gt.addEvent(new FatigueEvent(mover, path.attacking ? mover.movement : path.length));
            }
            handleTransaction(gt);
            #endregion

            gt = new GameTransaction();
            
            foreach (var v in paths)
            {
                Card mover = v.Item1;
                Path path = v.Item2;
                Card defender = path.to.card;

                if (path.attacking && mover.canAttack(defender))
                {
                    if (pathsx.Contains(v))
                    {
                        if (defender.canRetaliate && defender.toughness > 0)
                            gt.addEvent(new DamageEvent(defender, mover, defender.combatDamageTo(mover)));
                    }
                    else
                    {
                        gt.addEvent(new DamageEvent(mover, defender, mover.combatDamageTo(defender)));
                        if (defender.canRetaliate)
                            gt.addEvent(new DamageEvent(defender, mover, defender.combatDamageTo(mover)));
                    }
                }
                
                gt.addEvent(new MoveEvent(mover, path));
                gt.addEvent(new FatigueEvent(mover, path.attacking ? mover.movement : path.length));
            }
            foreach (var v in paths) v.Item1.tile.removeCard();
            handleTransaction(gt);

            foreach (var c in cards) c.combatPath = null;
            gameController.clearArrows();
            gameController.clearHighlights();
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

        LinkedList<GameEvent> gelog = new LinkedList<GameEvent>();

        private void logTransaction(GameTransaction t)
        {
            foreach (var v in t.events)
            {
                gelog.AddLast(v);
            }
        }

        private void handleTransaction(GameTransaction t)
        {
            logTransaction(t);
            var gameEvents = t.events;

            foreach (GameEvent e in gameEvents)
            {
                foreach (Card card in triggerableCards)
                {
                    pendAbilities(e, card.abilitiesTriggeredBy(e).Where(a => a.timing == TriggeredAbility.Timing.Pre));
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
                    pendAbilities(e, card.abilitiesTriggeredBy(e).Where(a => a.timing == TriggeredAbility.Timing.Post));
                }
            }
        }

        private void pendAbilities(GameEvent trigger, IEnumerable<TriggeredAbility> tas)
        {
            if (tas.Count() == 0) return;
            pendingTriggeredAbilities.AddRange(tas.Select(ta => new TriggerGlueHack(ta, trigger)));
        }

        private void enforceRules()
        {
            foreach (Card c in cards)
            {
                c.handleEvent(new ClearAurasEvent());
            }
            List<GameEvent> ae = new List<GameEvent>();
            foreach (Card c in cards)
            {
                foreach (Aura a in c.auras)
                {
                    if (c.location.pile != a.activeIn) continue;
                    foreach (Card affected in cards.Where(a.filter))
                    {
                        ae.Add(new ModifyEvent(affected, a.stat, a.modifer));
                    }
                }
            }

            foreach (Card c in cards)
            {
                foreach (GameEvent e in ae)
                {
                    c.handleEvent(e);
                }
            }

            foreach (Card c in cards)
            {
                c.updateState();
            }

            do
            {
                handlePendingTrigs();
                trashcanDeadCreatures();
            } while (pendingTriggeredAbilities.Count > 0);


            gameController.redraw();
        }

        private void handlePendingTrigs()
        {
            if (pendingTriggeredAbilities.Count > 0)
            {
                List<TriggerGlueHack>[] abilityArrays =
                    players.Select(p => new List<TriggerGlueHack>()).ToArray();

                foreach (TriggerGlueHack pending in pendingTriggeredAbilities)
                {
                    int ix = ord(pending.ta.card.controller);
                    abilityArrays[ix].Add(pending);
                }

                List<StackWrapper> wrappers = new List<StackWrapper>();

                for (int i = 0; i < abilityArrays.Length; i++)
                {
                    Player p = playerFromOrd(i);
                    List<TriggerGlueHack> abilityList = abilityArrays[i];
                    
                    wrappers.AddRange(handlePendingTrigs(p, abilityList));
                }

                pendingTriggeredAbilities.Clear();

                foreach (var w in wrappers)
                {
                    cast(w);
                }
            }
        }

        private List<StackWrapper> handlePendingTrigs(Player p, IEnumerable<TriggerGlueHack> abilities)
        {
            List<StackWrapper> r = new List<StackWrapper>();
            TriggerGlueHack[] orig = abilities.Where(a => a.ta.possible(makeHackStruct(a.ta))).ToArray();

            Pile pl = new Pile();

            for (int i = 0; i < orig.Length; i++)
            {
                Card c = createDummy(orig[i].ta, pl);
                c.tghack = orig[i];
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
                    int i = Array.IndexOf(orig.Select(g => g.ta).ToArray(), c.dummiedAbility);
                    TriggeredAbility tab = orig[i].ta;

                    dp.hide();
                    StackWrapper v = tryCastDx(p, tab, c, orig[i].ge);
                    dp.show();

                    if (v == null) continue;

                    ptas ptas = new ptas(tab,i,c,v);
                    triggd.Add(ptas);
                    cv.glowColour(Color.Green);
                }

                dp.close();

                foreach (ptas pt in triggd)
                {
                    r.Add(pt.wrapper);
                }

                connection.sendAction(new TriggeredAbilitiesGluer(
                    new ChoiceSelection(triggd.Select(t => pl.indexOf(t.dummyCard))),
                    triggd.Select(t => new CastSelection(t.wrapper)).ToArray()
                    ));
            }
            else
            {
                gameController.setPrompt("Opponent is handling triggered abilities.");

                TriggeredAbilitiesGluer glue = connection.receiveAction<TriggeredAbilitiesGluer>();

                var cs = glue.choices;
                var css = glue.castSelections;

                for (int i = 0; i < orig.Count(); i++)
                {
                    StackWrapper w = css[i].wrapper;
                    
                    Card dummy = pl[cs.choices[i]];
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
        private Tile getTile(Func<Tile, bool> f, string prompt, params ButtonOption[] buttons)
        {
            gameController.setPrompt(prompt, buttons);
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

                targetmxs = ability.effects.fillCast(makeHackStruct(ability));
                if (targetmxs == null) continue;

                costmxs = ability.cost.fillCast(makeHackStruct(p, ability));
                if (costmxs == null) continue;

                return new StackWrapper(card, ability, targetmxs, costmxs);
            } while (true);
        }

        private StackWrapper tryCastDx(Player p, TriggeredAbility ability, Card dummyCard, GameEvent ge)
        {
            TargetMatrix[] costmxs;
            TargetMatrix[] targetmxs;

            do
            {
                var v = makeHackStruct(ability);
                v.triggeringEvent = ge;
                targetmxs = ability.effects.fillCast(v);
                if (targetmxs == null) return null;

                costmxs = ability.cost.fillCast(makeHackStruct(p, ability));
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

            if (activatableAbilities.Length == 0) return null;
            if (activatableAbilities.Length > 1)
            {
                var v = activatableAbilities.Select(a => a.createDummy()).ToList();
                var sl = selectCardFromCards(v, true, 1, crd => true).ToArray();
                if (sl.Length == 0) return null;
                int i = v.IndexOf(sl[0]);
                r = activatableAbilities[i];
            }
            else
            {
                r = activatableAbilities[0];
            }

            if (!r.possible(makeHackStruct(r))) return null;

            return r;
        }

        private TargetMatrix[] chooseTargets(Ability a)
        {
            /*
            Card caster = a.isCastAbility ? a.card.controller.heroCard : a.card;
            List<Tile> v = caster.tile.withinDistance(a.castRange);
            */
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
                //gt.addEvent(new MoveToPileEvent(stackpopped, stackpopped.controller.field, false));
            }
            else
            {
                gt.addEvent(new MoveToPileEvent(stackpopped, stackpopped.owner.graveyard, false));
            }

            handleTransaction(gt);
        }

        public void forfeit(GameEndStateReason r)
        {
            connection.surrender(r);
        }

        public void endGame(GameEndStruct ras)
        {
            ScreenController.transtitionToPostGameScreen(this, ras);
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

        private HackStruct makeHackStruct(Player castingPlayer, Ability resolvingAbility)
        {
            return new HackStruct(this, resolvingAbility, castingPlayer);
        }

        public ManaColour selectManaColour(Player chooser, Func<ManaOrb, bool> f)
        {
            ManaOrbSelection selection;
            if (chooser == hero)
            {
                activePlayer.stuntMana();

                gameController.setPrompt("Gain mana nerd");
                ManaOrb v = (ManaOrb)waitForButtonOr<ManaOrb>(f);

                activePlayer.unstuntMana();

                selection = new ManaOrbSelection(v.colour);
                connection.sendAction(selection);
            }
            else
            {
                gameController.setPrompt("Opponent is gaining mana");
                selection = connection.receiveAction<ManaOrbSelection>();
            }
            return selection.orb;
        }

        private DraggablePanel showCards(IEnumerable<Card> cards, bool closeable)
        {
            return gameController.showCards(cards, closeable, clicked);
        }

        public IEnumerable<Card> selectCardFromCards(IEnumerable<Card> cards, bool cancelable, int cardCount, Func<Card, bool> filter)
        {
            List<Card> rt = new List<Card>(cardCount);
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


        public void enqueueGameMessage(string gmb)
        {
            connection.enqueueGameMessage(gmb);
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

    struct TriggerGlueHack
    {
        public TriggeredAbility ta;
        public GameEvent ge;

        public TriggerGlueHack(TriggeredAbility ta, GameEvent ge)
        {
            this.ta = ta;
            this.ge = ge;
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
