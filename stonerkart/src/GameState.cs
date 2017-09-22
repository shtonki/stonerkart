using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace stonerkart
{
    class GameState
    {
        public Map map { get; }
        public Player hero { get; }
        public Player villain { get; }
        public IEnumerable<Player> allPlayers => players;

        public List<Player> players;
        public List<Card> cards;

        public Pile stack { get; } = new Pile(new Location(null, PileLocation.Stack));

        public List<TriggerGlueHack> pendingTriggeredAbilities { get; } = new List<TriggerGlueHack>();

        public int activePlayerIndex { get; set; }
        public Player activePlayer => players[activePlayerIndex];

        public StepCounter stepCounter;

        public Stack<StackWrapper> wrapperStack { get; } = new Stack<StackWrapper>();

        public IEnumerable<Card> triggerableCards => cards.Where(card => card.location.pile != PileLocation.Deck && !card.isDummy);
        public IEnumerable<Card> fieldCards => cards.Where(card => card.location.pile == PileLocation.Field);

        public Random random;

        public GameStatistics statistics { get; private set; }= new GameStatistics();

        private GameEventHandlerBuckets baseHandler = new GameEventHandlerBuckets();

        public GameState(GameSetupInfo gsi)
        {
            //gameid = ngs.gameid;


            random = new Random(gsi.randSeed);
            this.map = map;
            cards = new List<Card>();
            players = new List<Player>();

            for (int i = 0; i < gsi.playerNames.Length; i++)
            {
                Player p = new Player(this, gsi.playerNames[i]);

                players.Add(p);
                if (gsi.playerNames[i] == Controller.user.Name) hero = p;
                else
                {
                    if (villain != null) throw new Exception();
                    villain = p;
                }
            }

            stepCounter = new StepCounter();

            setupHandlers();
        }

        private void setupHandlers()
        {
            baseHandler.add(new TypedGameEventHandler<MoveToPileEvent>(e =>
            {
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
                shuffle(e.player.deck);
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
            }));
        }
        
        public void shuffle(Pile p)
        {
            p.shuffle(random);
        }

        public int ord(Card c)
        {
            int v = cards.IndexOf(c);
            if (v == -1) throw new Exception();
            return v;
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

        public void handleTransaction(GameTransaction t)
        {
            var gameEvents = t.events;
            foreach (var e in gameEvents) statistics.LogEvent(e);

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

        public void pendAbilities(GameEvent trigger, IEnumerable<TriggeredAbility> tas)
        {
            if (tas.Count() == 0) return;
            pendingTriggeredAbilities.AddRange(tas.Select(ta => new TriggerGlueHack(ta, trigger)));
        }

    }

    interface GameLogger
    {
        void LogEvent(GameEvent e);
    }

    class GameStatistics : GameLogger
    {
        private List<GameEvent> events = new List<GameEvent>();

        public void LogEvent(GameEvent e)
        {
            events.Add(e);
        }

        public int totalDamageDealt()
        {
            return events.Where(e => e is DamageEvent).Cast<DamageEvent>().Sum(e => e.amount);
        }
    }
}
