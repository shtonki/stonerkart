using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class TargetRuleSet
    {
        public TargetRule[] rules;

        public TargetRuleSet(params TargetRule[] rules)
        {
            this.rules = rules;
        }

        public TargetMatrix fillCast(Func<Targetable> f)
        {
            TargetColumn[] r = new TargetColumn[rules.Length];

            for (int i = 0; i < rules.Length; i++)
            {
                TargetColumn? v = rules[i].fillCastTargets(f);
                if (v == null) return null;
                r[i] = v.Value;
            }

            return new TargetMatrix(r);
        }

        public TargetMatrix fillResolve(TargetMatrix tm, Card resolveCard, Game g)
        {
            TargetColumn[] r = tm.cs;

            for (int i = 0; i < rules.Length; i++)
            {
                r[i] = rules[i].fillResolveTargets(new ResolveEnv(resolveCard), r[i]);
            }

            return new TargetMatrix(r);
        }

    }

    class TargetMatrix
    {
        public TargetColumn[] cs;

        public TargetMatrix(TargetColumn[] cs)
        {
            this.cs = cs;
        }

        public TargetRow[] generateRows()
        {
            int l = cs.Aggregate(1, (current, c) => current*c.ts.Length);
            TargetRow[] r = new TargetRow[l];
            if (l != 1) throw new Exception();

            Targetable[] i1 = new Targetable[cs.Length];
            for (int i = 0; i < cs.Length; i++)
            {
                i1[i] = cs[i].ts[0];
            }

            r[0] = new TargetRow(i1);

            return r;
        }
    }

    interface TargetRule
    {
        TargetColumn? fillCastTargets(Func<Targetable> f);
        TargetColumn fillResolveTargets(ResolveEnv re, TargetColumn c);
    }

    class PryTileRule : TargetRule
    {
        private Func<Tile, bool> filter;

        public PryTileRule(Func<Tile, bool> filter)
        {
            this.filter = filter;
        }

        public TargetColumn? fillCastTargets(Func<Targetable> f)
        {
            while (true)
            {
                Targetable v = f();
                if (!(v is Tile)) continue;
                Tile t = (Tile)v;
                if (filter(t)) return new TargetColumn(t);
            }
        }

        public TargetColumn fillResolveTargets(ResolveEnv re, TargetColumn c)
        {
            return c;
        }
    }

    class PryCardRule : TargetRule
    {
        private Func<Card, bool> filter;

        public PryCardRule(Func<Card, bool> filter)
        {
            this.filter = filter;
        }

        public TargetColumn? fillCastTargets(Func<Targetable> f)
        {
            while (true)
            {
                Targetable v = f();
                if (!(v is Tile)) continue;
                Tile t = (Tile)v;
                if (t.card == null) continue;
                if (filter(t.card)) return new TargetColumn(t.card);
            }
        }

        public TargetColumn fillResolveTargets(ResolveEnv re, TargetColumn c)
        {
            return c;
        }
    }

    class ResolveRule : TargetRule
    {
        private Rule rule;

        public ResolveRule(Rule rule)
        {
            this.rule = rule;
        }

        public TargetColumn fillResolveTargets(ResolveEnv re, TargetColumn c)
        {
            if (c.ts.Length != 0) throw new Exception();
            switch (rule)
            {
                case Rule.ResolveCard:
                {
                    return new TargetColumn(re.resolveCard);
                }
            }
            throw new Exception();
        }

        public TargetColumn? fillCastTargets(Func<Targetable> f)
        {
            return new TargetColumn(new Targetable[] {});
        }

        public enum Rule
        {
            ResolveCard,
        }
    }

    struct ResolveEnv
    {
        public Card resolveCard;

        public ResolveEnv(Card resolveCard)
        {
            this.resolveCard = resolveCard;
        }
    }

    struct TargetRow
    {
        public Targetable[] ts;

        public TargetRow(Targetable[] ts)
        {
            this.ts = ts;
        }
    }
    

    struct TargetColumn
    {
        public Targetable[] ts;
        
        public TargetColumn(params Targetable[] ts)
        {
            this.ts = ts;
        }
    }
    
    
    interface Targetable
    {
        
    }
}
