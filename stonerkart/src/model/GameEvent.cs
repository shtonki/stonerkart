using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    interface GameEvent
    {
    }

    interface CardedEvent
    {
        Card getCard();
    }

    struct ApplyStacksEvent : GameEvent, CardedEvent
    {
        public Card card { get; }
        public Counter stack { get; }
        public int count { get; }

        public ApplyStacksEvent(Card card, Counter stack, int count)
        {
            this.card = card;
            this.stack = stack;
            this.count = count;
        }

        public Card getCard()
        {
            return card;
        }
    }

    struct FatigueEvent : GameEvent, CardedEvent
    {
        public Card card { get; }
        public int amount { get; }

        public FatigueEvent(Card card, int amount)
        {
            this.amount = amount;
            this.card = card;
        }

        public Card getCard()
        {
            return card;
        }
    }

    struct ClearAurasEvent : GameEvent
    {
        
    }

    struct GainBonusManaEvent : GameEvent
    {
        public Player player { get; }
        public ManaOrb orb { get; }

        public GainBonusManaEvent(Player player, ManaOrb orb)
        {
            this.player = player;
            this.orb = orb;
        }
    }

    struct ShuffleDeckEvent : GameEvent
    {
        public Player player { get; }

        public ShuffleDeckEvent(Player player)
        {
            this.player = player;
        }
    }

    struct ModifyEvent : GameEvent, CardedEvent
    {
        public Card card { get; }
        public ModifiableStats stat { get; }
        public ModifierStruct modifier { get; }

        public ModifyEvent(Card card, ModifiableStats stat, ModifierStruct modifier)
        {
            this.card = card;
            this.stat = stat;
            this.modifier = modifier;
        }

        public Card getCard()
        {
            return card;
        }
    }

    struct RaceModifyEvent : GameEvent, CardedEvent
    {
        public Card card { get; }
        public Race race => rmodifier.race;
        public GameEventFilter filter => rmodifier.filter;
        private RaceModifierStruct rmodifier { get; }

        public RaceModifyEvent(Card card, RaceModifierStruct rmodifier)
        {
            this.card = card;
            this.rmodifier = rmodifier;
        }

        public Card getCard()
        {
            return card;
        }
    }


    struct MoveToPileEvent : GameEvent, CardedEvent
    {
        public Pile to { get; }
        public Card card { get; }
        public bool nullTile { get; }

        public MoveToPileEvent(Card card, Pile to, bool nullTile = true)
        {
            this.to = to;
            this.card = card;
            this.nullTile = nullTile;
        }

        public Card getCard()
        {
            return card;
        }
    }

    struct DamageEvent : GameEvent, CardedEvent
    {
        public Card source { get; }
        public Card target { get; }
        public int amount { get; }

        public DamageEvent(Card source, Card target, int amount)
        {
            this.source = source;
            this.target = target;
            this.amount = amount;
        }

        public Card getCard()
        {
            return source;
        }
    }

    struct PlaceOnTileEvent : GameEvent, CardedEvent
    {
        public Card card { get; }
        public Tile tile { get; }
        public bool dontExhaust { get; }

        public PlaceOnTileEvent(Card card, Tile tile, bool dontExhaust)
        {
            this.card = card;
            this.tile = tile;
            this.dontExhaust = dontExhaust;
        }



        public Card getCard()
        {
            return card;
        }
    }

    struct MoveEvent : GameEvent, CardedEvent
    {
        public Card card { get; }
        public Path path { get; }

        public MoveEvent(Card card, Path path)
        {
            this.card = card;
            this.path = path;
        }

        public Card getCard()
        {
            return card;
        }
    }

    struct PayManaEvent : GameEvent
    {
        public ManaSet manaSet { get; }
        public Player player { get; }

        public PayManaEvent(Player player, ManaSet manaSet)
        {
            this.player = player;
            this.manaSet = manaSet;
        }
    }

    struct CastEvent : GameEvent
    {
        public StackWrapper wrapper { get; }

        public CastEvent(StackWrapper wrapper)
        {
            this.wrapper = wrapper;
        }
    }

    struct StartOfStepEvent : GameEvent
    {
        public Player activePlayer { get; }
        public Steps step { get; }

        public StartOfStepEvent(Player activePlayer, Steps step)
        {
            this.activePlayer = activePlayer;
            this.step = step;
        }
    }
    /*
    struct EndOfStepEvent : GameEvent
    {
        public Player activePlayer { get; }
        public Steps step { get; }

        public EndOfStepEvent(Player activePlayer, Steps step)
        {
            this.activePlayer = activePlayer;
            this.step = step;
        }
    }
    */
    struct DrawEvent : GameEvent
    {
        public Player player { get; }
        public int cards { get; }

        public DrawEvent(Player player, int cards)
        {
            this.player = player;
            this.cards = cards;
        }
    }
}
