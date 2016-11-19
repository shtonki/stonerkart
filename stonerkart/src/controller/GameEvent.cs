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

    class MoveEvent : GameEvent
    {
        public Card card { get; }
        public Tile tile { get; }

        public MoveEvent(Card card, Tile tile)
        {
            this.card = card;
            this.tile = tile;
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
        public Steps step { get; }

        public StartOfStepEvent(Steps step)
        {
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
