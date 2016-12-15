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
    
    class MoveToPileEvent : GameEvent
    {
        public Pile pile;
        public Card card;

        public MoveToPileEvent(Card card, Pile pile)
        {
            this.pile = pile;
            this.card = card;
        }
    }

    struct DamageEvent : GameEvent
    {
        public Card source;
        public Card target;
        public int amount;

        public DamageEvent(Card source, Card target, int amount)
        {
            this.source = source;
            this.target = target;
            this.amount = amount;
        }
    }

    struct PayCostsEvent : GameEvent
    {
        public Player player;
        public Ability ability;
        public int[][] costs;

        public PayCostsEvent(Player player, Ability ability, int[][] costs)
        {
            this.player = player;
            this.ability = ability;
            this.costs = costs;
        }
    }

    struct PlaceOnTileEvent : GameEvent
    {
        public Card card { get; }
        public Tile tile { get; }

        public PlaceOnTileEvent(Card card, Tile tile)
        {
            this.card = card;
            this.tile = tile;
        }
    }

    struct MoveEvent : GameEvent
    {
        public Card card { get; }
        public Path path { get; }

        public MoveEvent(Card card, Path path)
        {
            this.card = card;
            this.path = path;
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
        public Player activePlayer;
        public Steps step { get; }

        public StartOfStepEvent(Player activePlayer, Steps step)
        {
            this.activePlayer = activePlayer;
            this.step = step;
        }
    }

    struct EndOfStepEvent : GameEvent
    {
        public Player activePlayer;
        public Steps step { get; }

        public EndOfStepEvent(Player activePlayer, Steps step)
        {
            this.activePlayer = activePlayer;
            this.step = step;
        }
    }

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
