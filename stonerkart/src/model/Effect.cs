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

    class MoveToTileDoer : Doer
    {
        public GameEvent[] act(TargetRow[] ts)
        {
            List<GameEvent> r = new List<GameEvent>();

            foreach (var row in ts)
            {
                if (row.ts.Length != 2) throw new Exception();
                r.Add(new MoveToTileEvent((Card)row.ts[0], (Tile)row.ts[1]));
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
