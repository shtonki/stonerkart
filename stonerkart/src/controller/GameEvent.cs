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
