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
        private Func<Targetable, bool> filter;

        public CastRule(Func<Targetable, bool> filter)
        {
            this.filter = filter;
        }

        public Targetable[] fill(Func<Targetable> t)
        {
            while (true)
            {
                Targetable g = t();
                if (filter(g)) return new[] {g};
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
