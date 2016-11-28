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

        public MoveToPileEvent(Pile pile, Card card)
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

    struct AttackEvent : GameEvent
    {
        public Card attacker;
        public Card defender;
        public Path path;
        public Tile attackFrom;

        public AttackEvent(Path path)
        {
            this.attacker = path.from.card;
            this.defender = path.to.card;
            this.path = path;
            List<Tile> t = path;
            attackFrom = t[t.Count - 2];
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

    struct MoveToTileEvent : GameEvent
    {
        public Card card { get; }
        public Tile tile { get; }

        public MoveToTileEvent(Card card, Tile tile)
        {
            this.card = card;
            this.tile = tile;
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
        public Steps step { get; }

        public StartOfStepEvent(Steps step)
        {
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
