using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class TargetSet
    {
        public TargetRule[] ts;

        public TargetSet(params TargetRule[] ts)
        {
            this.ts = ts;
        }

        public TargetMatrix fillCast(Func<Targetable> generator)
        {
            Targetable[][] rt = new Targetable[ts.Length][];
            for (int i = 0; i < ts.Length; i++)
            {
                if (ts[i] is CastRule)
                {
                    Targetable[] v = ((CastRule)ts[i]).fill(generator);
                    if (v == null) return null;
                    rt[i] = v;
                }
                else
                {
                    rt[i] = null;
                }
            }
            return new TargetMatrix(rt);
        }

        public TargetMatrix fillResolve(TargetMatrix m, Card c, Game g)
        {
            Targetable[][] rt = m.ts;
            for (int i = 0; i < ts.Length; i++)
            {
                if (ts[i] is CastRule)
                {
                    if (rt[i] == null) throw new Exception();
                }
                else
                {
                    if (rt[i] != null) throw new Exception();
                    rt[i] = ((ResolveRule)ts[i]).fill(c, g);
                }
            }
            return new TargetMatrix(rt);
        }
    }

    interface TargetRule
    {
    }

    class CastRule : TargetRule
    {
        protected Func<Targetable, bool> filter;

        public CastRule(Func<Targetable, bool> filter)
        {
            this.filter = filter;
        }

        public virtual Targetable[] fill(Func<Targetable> t)
        {
            while (true)
            {
                Targetable g = t();
                if (g == null)
                {
                    return null;
                }
                if (filter(g)) return new[] {g};
            }
        }
    }

    class PryRule<T> : CastRule where T : Targetable
    {
        public PryRule(Func<T, bool> filter) : base(t => t is T && filter((T)t))
        {
        }

        public override Targetable[] fill(Func<Targetable> t)
        {
            while (true)
            {
                Targetable g = t();
                
                if (!(g is Tile)) continue;

                Tile tile = (Tile)g;
                Card c = tile.card;
                if (c == null) continue;
                if (filter(c)) return new[] { (Targetable)c };
                if (c.cardType != CardType.Hero) continue;
                Player p = c.owner;
                if (filter(p)) return new[] { (Targetable)p };
            }
        }
    }

    class ResolveRule : TargetRule
    {
        private Rule rule;

        public ResolveRule(Rule rule)
        {
            this.rule = rule;
        }

        public Targetable[] fill(Card resolvingCard, Game g)
        {
            switch (rule)
            {
                case Rule.CastCard:
                {
                    return new Targetable[] {resolvingCard,};
                } break;

                default:
                    throw new Exception();
            }
        }

        public enum Rule
        {
            CastCard,

        }
    }
    
    class TargetMatrix
    {
        public Targetable[][] ts;

        public TargetMatrix(Targetable[][] ts)
        {
            this.ts = ts;
        }
    }
    
    interface Targetable
    {
        
    }
}
