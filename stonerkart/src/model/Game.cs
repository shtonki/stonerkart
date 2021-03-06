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
        public GameController gameController;

        public int gameid { get; }
        
        private GameConnection connection;

        public GameState gameState;

        private bool skipFirstDraw = true;

        private GameScreen screen;

        private const string waiterPrompt = "Waiting for opponent.";

        public Game(NewGameStruct ngs, bool local, GameScreen gameScreen)
        {
            screen = gameScreen;

            gameid = ngs.gameid;
            
            gameController = new GameController(null, null);

            gameState = new GameState(ngs);

            if (local)
            {
                connection = new DummyConnection();
            }
            else
            {
                connection = new MultiplayerConnection(this, ngs);
            }

            observe();
        }

        private void observe()
        {
            gameState.hero.hand.addObserver(screen.handView);
            gameState.hero.manaPool.addObserver(screen.heroPanel);

            gameState.villain.manaPool.addObserver(screen.villainPanel);

            gameState.stack.addObserver(screen.stackWinduh);

            gameState.hero.field.addObserver(screen.hexPanel);
            gameState.villain.field.addObserver(screen.hexPanel);

            gameState.hero.graveyard.addObserver(screen.heroGraveyard);
            screen.heroGraveyardWinduh.Title = String.Format("{0}'s Graveyard", gameState.hero.name);

            gameState.hero.hand.addObserver(screen.heroPanel);
            gameState.hero.deck.addObserver(screen.heroPanel);
            gameState.hero.graveyard.addObserver(screen.heroPanel);
            gameState.hero.displaced.addObserver(screen.heroPanel);

            gameState.villain.hand.addObserver(screen.villainPanel);
            gameState.villain.deck.addObserver(screen.villainPanel);
            gameState.villain.graveyard.addObserver(screen.villainPanel);
            gameState.villain.displaced.addObserver(screen.villainPanel);
        }

        public Card createToken(CardTemplate ct, Player owner)
        {
            Card r = createCard(ct, owner.field, owner);
            if (!r.isToken) throw new Exception();
            return r;
        }

        private Card createCard(CardTemplate ct, Player owner)
        {
            Card r = new Card(ct, owner);
            gameState.cards.Add(r);
            return r;
        }

        private Card createCard(CardTemplate ct, Pile pile, Player owner)
        {
            Card r = createCard(ct, owner);
            r.moveTo(pile);
            return r;
        }

        private Card createDummy(Ability a)
        {
            Card r = a.createDummy();
            gameState.cards.Add(r);
            return r;
        }

        public Path pathTo(Card c, Tile t)
        {

            throw new NotImplementedException("if you weren't expecting too see this you might be in some trouble son");/*
            return map.path(c, t);*/
        }

        public void startGameThread()
        {
            Thread t = new Thread(gameLoop);
            t.Start();
        }

        private void initGame()
        {
            Deck d = DeckController.chooseDeck();

            Deck[] decks = connection.deckify(d, gameState.ord(gameState.hero));

            for (int i = 0; i < gameState.players.Count; i++)
            {
                Player p = gameState.players[i];
                Deck deck = decks[i];

                Card heroCard = createCard(deck.hero, p.field, p);
                p.setHeroCard(heroCard);

                //var cards = deck.templates.Select(ct => createCard(ct, p.deck, p));
                //foreach (var card in cards) card.moveTo(p.deck);
                //p.deck.addRange(cards);
                
                foreach (var ct in deck.templates)
                {
                    createCard(ct, p.deck, p);
                }
                
                gameState.shuffle(p.deck);
            }


            Tile[] ts = new[]
            {
                gameState.map.tileAt(2, 2),
                gameState.map.tileAt(gameState.map.width - 3, gameState.map.height - 3),
            };
            int ix = 0;
            foreach (Player p in gameState.players)
            {
                if (ix >= ts.Length) throw new Exception();

                ts[ix++].place(p.heroCard);
            }

            int xi = gameState.random.Next();

            if (connection is DummyConnection) xi = gameState.ord(gameState.hero);
            Player flipwinner = gameState.players[xi % gameState.players.Count];
            ButtonOption bopt = chooseButtonSynced(flipwinner,
                "Do you wish to go first?",
                ButtonOption.Yes, ButtonOption.No);
            
            gameState.activePlayerIndex = (xi + (bopt == ButtonOption.Yes ? 0 : 1)) % gameState.players.Count;

            int[] draws = { 5, 5, 4, 3, 2, 1 };

            for (int i = 0; i < gameState.players.Count; i++)
            {
                for (int j = 0; j < draws.Length; j++)
                {
                    Player p = gameState.players[(i + gameState.activePlayerIndex) % gameState.players.Count];
                    int cards = draws[j];
                    handleTransaction(new DrawEvent(p, cards));

                    if (j == draws.Length - 1) break;

                    var bo = chooseButtonSynced(p,
                        String.Format("Redraw to {0}?", draws[j + 1]),
                        ButtonOption.Yes, ButtonOption.No);

                    if (bo == ButtonOption.Yes)
                    {
                        while (p.hand.Count > 0)
                        {
                            Card crd = p.hand.peek();
                            crd.moveTo(p.deck);
                        }
                        gameState.shuffle(p.deck);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        private void gameLoop()
        {
            gameController.setPrompt("Game starting");
            initGame();

            while (true)
            {
                screen.turnIndicator.setActive(gameState.stepCounter.step, gameState.activePlayer.isHero);
                doStep(gameState.stepCounter.step);
                gameState.stepCounter.nextStep();
            }
        }

        private void doStep(Steps step)
        {
            handleTransaction(new StartOfStepEvent(gameState.activePlayer, step));
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

            handleTransaction(new EndOfStepEvent(gameState.activePlayer, step));

            foreach (Player p in gameState.players)
            {
                p.clearBonusMana();
            }
        }

        private void untapStep()
        {
            gameState.activePlayer.resetMana();

            if (!skipFirstDraw)
            {
                handleTransaction(new DrawEvent(gameState.activePlayer, 1));
            }
            else
            {
                skipFirstDraw = false;
            }

            if (gameState.activePlayer.manaPool.maxCount < 12)
            {
                Player addingPlayer = gameState.activePlayer;
                var mc = chooseManaColourSynced(addingPlayer, o => addingPlayer.manaPool.currentMana(o) != 6,
                    "Choose which mana colour to add to your mana pool.");
                gameState.activePlayer.gainMana(mc.Value);
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

        public void highlight(IEnumerable<Targetable> ts, Color c)
        {
            List<Tile> tiles = new List<Tile>();
            foreach (var t in ts)
            {
                if (t is Tile) tiles.Add((Tile)t);
                if (t is Card)
                {
                    Card card = (Card)t;
                    if (card.location.pile == PileLocation.Field) tiles.Add(card.tile);
                }
                if (t is Player) tiles.Add(((Player)t).heroCard.tile);
            }

            highlight(tiles, c);
        }

        public void highlight(IEnumerable<Tile> ts, Color c)
        {
            foreach (var t in ts) screen.hexPanel.highlight(t.x, t.y, c);
        }

        public void clearHighlights()
        {
            screen.hexPanel.clearHighlights();
        }

        private void moveStep()
        {
            List<Tuple<Card, Path>> paths;

            Player movingPlayer = gameState.activePlayer;
            var movers = movingPlayer.field.Where(c => c.canMove);

            var unpathed = movers.ToList();
            var occupado = movingPlayer.field.Where(c => !c.canMove).Select(c => c.tile).ToList();

            while (true)
            {
                highlight(unpathed.Select(c => c.tile), Color.DodgerBlue);
                //gameController.highlight(pathed.Select(c => c.tile), Color.ForestGreen);

                Tile from = chooseTileSynced(
                    movingPlayer, 
                    tile => tile.card != null && tile.card.canMove && tile.card.owner == movingPlayer,
                    "Choose what to move.", 
                    unpathed.Count == 0 ? ButtonOption.OK : (ButtonOption?)null);
                if (from == null) break;
                Card mover = from.card;
                if (mover.combatPath != null)
                {
                    removeArrow(mover.combatPath);
                    occupado.Remove(mover.combatPath.last);
                }
                Path pth = new Path(from);
                pth.colorHack = clrs[gameState.ord(mover)%clrs.Length];
                addArrow(pth);

                while (true)
                {

                    var options = gameState.map.pathCosts(mover, from).Where(path =>
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
                    var v = gameState.map.tyles.Where(t => t.card?.controller == mover.controller).ToArray();
                    if (movingPlayer.isHero) highlight(options.Select(p => p.to), Color.Green);
                    Tile to = chooseTileSynced(
                        movingPlayer, 
                        tile => options.Select(p => p.to).Contains(tile),
                        "Choose where to move.", 
                        ButtonOption.Cancel);
                    clearHighlights();
                    if (to == null) continue;
                    if (to == from) break;

                    var cp = options.First(p => p.to == to);

                    removeArrow(pth);
                    pth.concat(cp);
                    addArrow(pth);
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

            var x = paths.Select(p => p.Item1.moveCount).ToArray();
            //todo raise MoveDeclared events lmfao
            priority();
            paths = paths.Where((p, i) => p.Item1.moveCount == x[i]).ToList();

            GameTransaction gt = new GameTransaction();
            
            foreach (var v in paths)
            {
                Card mover = v.Item1;
                Path path = v.Item2;
                Card defender = path.to.card;

                if (path.attacking && mover.canAttack(defender))
                {
                    gt.addEvent(new DamageEvent(mover, defender, mover.combatDamageTo(defender)));

                    if (!mover.hasAbility(KeywordAbility.Ambush) && defender.canRetaliate)
                    {
                        gt.addEvent(new DamageEvent(defender, mover, defender.combatDamageTo(mover)));
                    }
                }
                
                gt.addEvent(new MoveEvent(mover, path));
                gt.addEvent(new FatigueEvent(mover, path.attacking ? mover.movement : path.length));
            }
            foreach (var v in paths) v.Item1.tile.removeCard();
            handleTransaction(gt);

            foreach (var c in gameState.cards) c.combatPath = null;
            clearArrows();
            clearHighlights();
        }

        private void endStep()
        {
            priority();

            gameState.activePlayerIndex = (gameState.activePlayerIndex + 1)% gameState.players.Count;
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
            gameState.handleTransaction(t);
        }

        private void enforceRules()
        {
            foreach (var player in gameState.players)
            {
                player.field.removeWhere(c => c.isToken && c.tile == null);
            }

            foreach (Card c in gameState.cards)
            {
                c.handleEvent(new ClearAurasEvent());
            }
            List<GameEvent> ae = new List<GameEvent>();
            foreach (Card c in gameState.cards)
            {
                foreach (Aura a in c.auras)
                {
                    if (c.location.pile != a.activeIn) continue;
                    foreach (Card affected in gameState.cards.Where(a.filter))
                    {
                        ae.Add(new ModifyEvent(affected, a.stat, a.modifer));
                    }
                }
            }

            foreach (Card c in gameState.cards)
            {
                foreach (GameEvent e in ae)
                {
                    c.handleEvent(e);
                }
            }

            foreach (Card c in gameState.cards)
            {
                c.updateState();
            }

            do
            {
                handlePendingTrigs();
                trashcanDeadCreatures();
            } while (gameState.pendingTriggeredAbilities.Count > 0);
        }

        private void handlePendingTrigs()
        {
            if (gameState.pendingTriggeredAbilities.Count > 0)
            {
                List<TriggerGlueHack>[] abilityArrays =
                    gameState.players.Select(p => new List<TriggerGlueHack>()).ToArray();

                foreach (TriggerGlueHack pending in gameState.pendingTriggeredAbilities)
                {
                    int ix = gameState.ord(pending.ta.card.controller);
                    abilityArrays[ix].Add(pending);
                }

                for (int i = 0; i < abilityArrays.Length; i++)
                {
                    Player p = gameState.playerFromOrd(i);
                    List<TriggerGlueHack> abilityList = abilityArrays[i];
                    
                    handlePendingTrigs(p, abilityList);
                }

                gameState.pendingTriggeredAbilities.Clear();
            }
        }

        private void handlePendingTrigs(Player triggerOwner, IEnumerable<TriggerGlueHack> abilities)
        {
            TriggerGlueHack[] orig = abilities.ToArray();

            CardList dummyCards = new CardList();

            for (int i = 0; i < orig.Length; i++)
            {
                Card c = createDummy(orig[i].ta);
                dummyCards.addTop(c);
                c.tghack = orig[i];
            }

            List<ptas> triggd;

            do
            {
                triggd = orderTriggers(dummyCards, triggerOwner, orig);
            } while (triggd == null);
            

            foreach (ptas pt in triggd)
            {
                cast(pt.wrapper, pt.costEvents);
            }
        }

        private List<ptas> orderTriggers(CardList cards, Player triggerOwner, TriggerGlueHack[] orig)
        {
            List<ptas> triggd = new List<ptas>();

            var handled = new CardList();

            while (handled.Count < cards.Count)
            {
                Card dummyCard = chooseCardsFromCardsSynced(
                    triggerOwner, 
                    cards.Except(handled), _ => true,
                    "Choose which ability to place on the stack next.",
                    ButtonOption.Cancel,
                    String.Format("{0}'s Triggered Abilities", triggerOwner.name));

                if (dummyCard == null) return null;

                int i = Array.IndexOf(orig.Select(g => g.ta).ToArray(), dummyCard.dummiedAbility);

                TriggeredAbility ability = orig[i].ta;
                var triggeringEvent = orig[i].ge;

                var hs = new HackStruct(this, ability);
                hs.triggeringEvent = triggeringEvent;

                var costTargets = ability.targetCosts(hs);
                if (costTargets.Fizzled) { handled.addTop(dummyCard); continue; }
                if (costTargets.Cancelled) return null;
                var payCostGameEvents = ability.payCosts(hs, costTargets);

                var castTargets = ability.targetCast(hs);
                if (castTargets.Fizzled) { handled.addTop(dummyCard); continue; }
                if (castTargets.Cancelled) return null;

                StackWrapper wrapper = new StackWrapper(dummyCard, ability, castTargets);

                ptas ptas = new ptas(ability, dummyCard, wrapper, payCostGameEvents);
                triggd.Add(ptas);
                handled.addTop(dummyCard);
            }

            return triggd;
        }

        #region hack
        private class ptas
        {
            public TriggeredAbility ability { get; }
            public Card dummyCard { get; }
            public StackWrapper wrapper { get; }
            public IEnumerable<GameEvent> costEvents { get; }

            public ptas(TriggeredAbility ability, Card dummyCard, StackWrapper wrapper, IEnumerable<GameEvent> costEvents)
            {
                this.ability = ability;
                this.dummyCard = dummyCard;
                this.wrapper = wrapper;
                this.costEvents = costEvents;
            }
        }
        #endregion 

        private void trashcanDeadCreatures()
        { 
            List<Card> trashcan = new List<Card>();
            foreach (Card c in gameState.fieldCards)
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
                Player playerWithPriority = gameState.players[(gameState.activePlayerIndex + c)% gameState.players.Count];

                bool playerCastSomething = givePriority(playerWithPriority);

                if (playerCastSomething)
                {
                    c = 0;
                }
                else //pass
                {
                    c++;
                    if (c == gameState.players.Count) //if it was passed all the way around
                    {
                        if (gameState.wrapperStack.Count == 0)
                        {
                            return;
                        }
                        else
                        {
                            resolveTopCardOnStack();
                            c = 0;
                        }
                    }
                }
            }
        }

        private void resolveTopCardOnStack()
        {
            StackWrapper wrapper = gameState.wrapperStack.Pop();
            if (wrapper.castingCard != gameState.stack.peekTop() && wrapper.castingCard != gameState.stack.peekTop().dummyFor) throw new Exception();
            resolve(wrapper);
        }
        
        private void cast(StackWrapper w, IEnumerable<GameEvent> costEvents)
        {
            if (!w.isCastAbility)
            {
                w = new StackWrapper(w.ability.createDummy(), w.ability, w.cachedTargets);
            }

            GameTransaction gt = new GameTransaction();
            gt.addEvents(costEvents);
            handleTransaction(gt);

            gt = new GameTransaction();
            gt.addEvent(new CastEvent(w));
            handleTransaction(gt);
        }

        /// <summary>
        /// Gives a player priority.
        /// </summary>
        /// <param name="playerWithPriority"></param>
        /// <returns>True if the player cast a spell or used an ability, false if he passed. </returns>
        private bool givePriority(Player playerWithPriority)
        { 
            //gameController.setHeroActive(b);

            do
            {

                /*
                if (!(gameState.stack.Any() ||
                        Settings.stopTurnSetting.getTurnStop(gameState.stepCounter.step, gameState.hero == gameState.activePlayer)))
                {
                    return null;    //auto pass
                }
                */

                var card = chooseCardSynced(
                    playerWithPriority, 
                    c => c.controller == playerWithPriority,
                    "You may cast a card.",
                    ButtonOption.Pass);
                if (card == null) return false;

                var ability = chooseAbilitySynced(card);
                if (ability == null) continue;

                HackStruct hs = new HackStruct(this, ability, playerWithPriority);

                var costTargets = ability.targetCosts(hs);
                if (costTargets.Cancelled || costTargets.Fizzled) continue;

                var payCostGameEvents = ability.payCosts(hs, costTargets);
                if (payCostGameEvents == null) continue;

                var targets = ability.targetCast(hs);
                if (targets.Cancelled || targets.Fizzled) continue;

                //if (!ability.isCastAbility) throw new NotImplementedException("if you weren't expecting too see this you might be in some trouble son");

                StackWrapper wrapper = new StackWrapper(card, ability, targets);

                cast(wrapper, payCostGameEvents);
                return true;
            } while (true);
        }

        private ActivatedAbility chooseAbilitySynced(Card c)
        {
            ActivatedAbility r;
            ActivatedAbility[] activatableAbilities = c.usableHere;

            bool canCastSlow = gameState.stack.Count == 0 && gameState.activePlayer == c.controller && (gameState.stepCounter.step == Steps.Main1 || gameState.stepCounter.step == Steps.Main2);
            if (!canCastSlow)
            {
                activatableAbilities = activatableAbilities.Where(a => a.isInstant).ToArray();
            }

            if (activatableAbilities.Length == 0) return null;
            if (activatableAbilities.Length > 1 || c.location.pile == PileLocation.Field)
            {
                var dummyCards = activatableAbilities.Select(a => a.createDummy()).ToList();
                var selectedCard = chooseCardsFromCardsSynced(
                    c.controller, 
                    dummyCards, _ => true, 
                    "Choose which ability to activate.", 
                    ButtonOption.Cancel);
                //var sl = selectCardFromCards(v, true, 1, crd => true).ToArray();
                //if (sl.Length == 0) return null;
                //int i = v.IndexOf(sl[0]);
                //r = activatableAbilities[i];
                if (selectedCard == null) return null;
                r = (ActivatedAbility)selectedCard.dummiedAbility;
            }
            else
            {
                r = activatableAbilities[0];
            }

            //if (!r.possible(makeHackStruct(r))) return null;

            return r;
        }

        private void resolve(StackWrapper wrapper)
        {
            Card stackpopped = gameState.stack.peekTop();
            Card card = wrapper.castingCard;
            Ability ability = wrapper.ability;
            if (stackpopped.isDummy && stackpopped.dummyFor != card) throw new Exception();

            HackStruct hs = new HackStruct(this, ability, card.controller);

            //fill resolve targets first
            //throw new NotImplementedException("if you weren't expecting too see this you might be in some trouble son");

            var finalTargets = ability.targetResolve(hs, wrapper.cachedTargets);
            if (finalTargets.Cancelled) throw new Exception();

            IEnumerable<GameEvent> resolveEvents;
            if (finalTargets.Fizzled)
            {
                resolveEvents = new List<GameEvent>();
            }
            else
            {
                resolveEvents = ability.resolve(hs, finalTargets);
            }

            if (resolveEvents == null) throw new NotImplementedException("if you weren't expecting too see this you might be in some trouble son");
            GameTransaction gt = new GameTransaction(resolveEvents);

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
            throw new NotImplementedException("if you weren't expecting too see this you might be in some trouble son");/*
            ScreenController.transtitionToPostGameScreen(this, ras);*/
        }

        private Card chooseCardUnsynced(Func<Card, bool> filter)
        {
            PublicSaxophone sax = new PublicSaxophone(o =>
            {
                if (o is ButtonOption) return true;

                if (o is Card)
                {
                    Card card = (Card)o;
                    return filter(card);
                }

                if (o is Tile)
                {
                    Tile tile = (Tile)o;

                    return tile.card != null && filter(tile.card);
                }

                return false;
            });

            screen.promptPanel.sub(sax);
            screen.handView.sub(sax);
            screen.stackView.sub(sax);
            screen.hexPanel.subTile(sax, gameState.map.tileAt);

            var v = sax.call();
            if (v is ButtonOption)
            {
                return null;
            }
            if (v is Card)
            {
                return (Card)v;
            }
            if (v is Tile)
            {
                return ((Tile)v).card;
            }

            throw new Exception();
        }

        public Card chooseCardSynced(Player chooser, Func<Card, bool> filter, string chooserPrompt, ButtonOption? button = null)
        {
            Card rt;
            if (chooser.isHero)
            {
                screen.promptPanel.prompt(chooserPrompt);
                if (button.HasValue) screen.promptPanel.promptButtons(button.Value);
                rt = chooseCardUnsynced(filter);
                if (rt == null) connection.sendChoice(null);
                else connection.sendChoice(gameState.ord(rt));
            }
            else
            {
                screen.promptPanel.prompt(waiterPrompt);
                int? choice = connection.receiveChoice();
                if (choice.HasValue) rt = gameState.cardFromOrd(choice.Value);
                else rt = null;
            }
            return rt;
        }

        public ButtonOption chooseButtonSynced(Player chooser, string chooserPrompt, params ButtonOption[] options)
        {
            ButtonOption rt;
            if (chooser.isHero)
            {
                screen.promptPanel.prompt(chooserPrompt);
                screen.promptPanel.promptButtons(options);
                rt = chooseButtonUnsynced(options);
                connection.sendChoice((int)rt);
            }
            else
            {
                screen.promptPanel.prompt(waiterPrompt);
                var choice = connection.receiveChoice();
                rt = (ButtonOption)choice;
            }

            return rt;
        }

        private ButtonOption chooseButtonUnsynced(params ButtonOption[] options)
        {
            PublicSaxophone sax = new PublicSaxophone(o => o is ButtonOption);
            screen.promptPanel.sub(sax);
            var v = (ButtonOption)sax.call();
            return v;
        }

        public Tile chooseTileSynced(Player chooser, Func<Tile, bool> filter, string chooserPrompt, ButtonOption? button = null)
        {
            Tile rt;
            if (chooser.isHero)
            {
                screen.promptPanel.prompt(chooserPrompt);
                if (button.HasValue) screen.promptPanel.promptButtons(button.Value);
                rt = chooseTileUnsynced(filter);
                if (rt == null) connection.sendChoice(null);
                else connection.sendChoice(gameState.ord(rt));
            }
            else
            {
                screen.promptPanel.prompt(waiterPrompt);
                var choice = connection.receiveChoice();
                if (choice.HasValue) rt = gameState.tileFromOrd(choice.Value);
                else rt = null;
            }
            return rt;
        }

        private Tile chooseTileUnsynced(Func<Tile, bool> filter)
        {
            PublicSaxophone sax = new PublicSaxophone(o => o is ButtonOption || (o is Tile && filter((Tile)o)));
            screen.hexPanel.subTile(sax, gameState.map.tileAt);
            screen.promptPanel.sub(sax);
            var v = sax.call();

            if (v is ButtonOption) return null;
            return (Tile)v;
        }

        public ManaColour? chooseManaColourSynced(Player chooser, Func<ManaColour, bool> filter, string chooserPrompt, ButtonOption? button = null)
        {
            ManaColour? clr;
            if (chooser == gameState.hero)
            {
                ManaColour[] cs = new[]
                {
                    ManaColour.Chaos, ManaColour.Death, ManaColour.Life, ManaColour.Might, ManaColour.Nature, ManaColour.Order,
                };
                screen.promptPanel.prompt(chooserPrompt);
                if (button.HasValue) screen.promptPanel.promptButtons(ButtonOption.NOTHING, ButtonOption.NOTHING, button.Value);
                screen.promptPanel.promptManaChoice(!button.HasValue, cs.Where(filter).ToArray());
                clr = chooseManaColourUnsynced(filter);
                if (!clr.HasValue) connection.sendChoice(null);
                else connection.sendChoice((int)clr.Value);
            }
            else
            {
                screen.promptPanel.prompt(waiterPrompt);
                var received = connection.receiveChoice();
                if (received.HasValue) clr = (ManaColour)received;
                else clr = null;
            }
            return clr;
        }

        private ManaColour? chooseManaColourUnsynced(Func<ManaColour, bool> filter)
        {
            PublicSaxophone sax = new PublicSaxophone(o => o is ButtonOption || (o is ManaColour && filter((ManaColour)o)));
            screen.promptPanel.sub(sax);
            var v = sax.call();
            if (v is ButtonOption) return null;
            if (v is ManaColour) return (ManaColour)v;
            throw new Exception();
        }

        public Card chooseCardsFromCardsSynced(Player chooser, IEnumerable<Card> cards, Func<Card, bool> filter,
            string chooserPrompt, ButtonOption? button, string title = "")
        {
            List<Card> cardList = cards.ToList();
            Card rt;
            if (chooser.isHero)
            {
                screen.promptPanel.prompt(chooserPrompt);
                if (button.HasValue) screen.promptPanel.promptButtons(button.Value);
                rt = chooseCardsFromCardsUnsynced(cardList, filter, title);
                if (rt == null) connection.sendChoice(null);
                else connection.sendChoice(cardList.IndexOf(rt));
            }
            else
            {
                screen.promptPanel.prompt(waiterPrompt);
                var cs = connection.receiveChoice();
                if (cs.HasValue)
                {
                    rt = cardList[cs.Value];
                }
                else
                {
                    rt = null;
                }
            }
            return rt;
        }

        private Card chooseCardsFromCardsUnsynced(IEnumerable<Card> cards, Func<Card, bool> filter, string title)
        {
            PileView pv = new PileView(600, 300, cards);
            pv.Backcolor = Color.Silver;
            Winduh window = new Winduh(pv);
            window.Title = title;
            screen.addWinduh(window);

            PublicSaxophone sax = new PublicSaxophone(o => o is ButtonOption || (o is Card) && filter((Card)o));
            pv.sub(sax);
            screen.promptPanel.sub(sax);

            var v = sax.call();

            screen.removeElement(window);

            if (v is ButtonOption) return null;
            if (v is Card) return (Card)v;
            throw new Exception();
        }


        public void addArrow(Path l)
        {
            screen.hexPanel.addPath(l);
        }

        public void removeArrow(Path l)
        {
            screen.hexPanel.removeArrow(l);
        }

        public void clearArrows()
        {
            screen.hexPanel.clearPaths();
        }

        public void clearTargetHighlight()
        {
            clearHighlights();
        }

        public void setTargetHighlight(Card c)
        {
            throw new NotImplementedException("if you weren't expecting too see this you might be in some trouble son");
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
        public TargetMatrix cachedTargets { get; }


        public Card castingCard => stackCard.isDummy ? stackCard.dummyFor : stackCard;
        public bool isCastAbility => castingCard.isCastAbility(ability);

        public StackWrapper(Card stackCard, Ability ability, TargetMatrix cachedTargets)
        {
            this.stackCard = stackCard;
            this.ability = ability;
            this.cachedTargets = cachedTargets;
        }
    }


    class StepCounter : Observable<Steps>
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
