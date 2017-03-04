using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class GameEvent
    {
        public int id { get; }
        private static int idCtr = 0;

        public GameEvent()
        {
            id = idCtr++;
        }
    }

    interface CardedEvent
    {
        Card getCard();
    }

    class GainBonusManaEvent : GameEvent
    {
        public Player player;
        public ManaColour colour;

        public GainBonusManaEvent(Player player, ManaColour colour)
        {
            this.player = player;
            this.colour = colour;
        }
    }

    class ShuffleDeckEvent : GameEvent
    {
        public Player player;

        public ShuffleDeckEvent(Player player)
        {
            this.player = player;
        }
    }

    class MoveToPileEvent : GameEvent, CardedEvent
    {
        public Pile to;
        public Card card;
        public bool nullTile;

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

    class DamageEvent : GameEvent, CardedEvent
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

        public Card getCard()
        {
            return source;
        }
    }

    class PlaceOnTileEvent : GameEvent, CardedEvent
    {
        public Card card { get; }
        public Tile tile { get; }

        public PlaceOnTileEvent(Card card, Tile tile)
        {
            this.card = card;
            this.tile = tile;
        }

        public Card getCard()
        {
            return card;
        }
    }

    class MoveEvent : GameEvent, CardedEvent
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

    class PayManaEvent : GameEvent
    {
        public ManaSet manaSet { get; }
        public Player player;

        public PayManaEvent(Player player, ManaSet manaSet)
        {
            this.player = player;
            this.manaSet = manaSet;
        }
    }

    class CastEvent : GameEvent
    {
        public StackWrapper wrapper { get; }

        public CastEvent(StackWrapper wrapper)
        {
            this.wrapper = wrapper;
        }
    }

    class StartOfStepEvent : GameEvent
    {
        public Player activePlayer;
        public Steps step { get; }

        public StartOfStepEvent(Player activePlayer, Steps step)
        {
            this.activePlayer = activePlayer;
            this.step = step;
        }
    }

    class EndOfStepEvent : GameEvent
    {
        public Player activePlayer;
        public Steps step { get; }

        public EndOfStepEvent(Player activePlayer, Steps step)
        {
            this.activePlayer = activePlayer;
            this.step = step;
        }
    }

    class DrawEvent : GameEvent
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
