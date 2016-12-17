using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class Effect
    {
        public TargetRuleSet ts;
        public Doer doer;

        public Effect(TargetRuleSet ts, Doer d)
        {
            this.ts = ts;
            doer = d;
        }
    }

    internal interface Doer
    {
        GameEvent[] act(TargetRow[] ts);
    }

    class DrawCardsDoer : Doer
    {
        public int cards;

        public DrawCardsDoer(int cards)
        {
            this.cards = cards;
        }

        public GameEvent[] act(TargetRow[] ts)
        {
            List<GameEvent> r = new List<GameEvent>();

            foreach (var row in ts)
            {
                if (row.ts.Length != 1 || !(row.ts[0] is Player)) throw new Exception();
                Player player = (Player)row.ts[0];
                r.Add(new DrawEvent(player, cards));
            }

            return r.ToArray();
        }
    }

    class MoveToTileDoer : Doer
    {
        public GameEvent[] act(TargetRow[] ts)
        {
            List<GameEvent> r = new List<GameEvent>();

            foreach (var row in ts)
            {
                if (row.ts.Length != 2) throw new Exception();
                r.Add(new PlaceOnTileEvent((Card)row.ts[0], (Tile)row.ts[1]));
            }

            return r.ToArray();
        }
    }

    class ZepperDoer : Doer
    {
        public int damage;

        public ZepperDoer(int damage)
        {
            this.damage = damage;
        }

        public GameEvent[] act(TargetRow[] ts)
        {
            List<GameEvent> r = new List<GameEvent>();

            foreach (var row in ts)
            {
                if (row.ts.Length != 2) throw new Exception();
                r.Add(new DamageEvent((Card)row.ts[0], (Card)row.ts[1], damage));
            }

            return r.ToArray();
        }
    }
    
}
