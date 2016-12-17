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

        public TargetMatrix fillCast(ChooseTargetToolbox str)
        {
            TargetColumn[] r = new TargetColumn[rules.Length];

            for (int i = 0; i < rules.Length; i++)
            {
                TargetColumn? v = rules[i].fillCastTargets(str);
                if (v == null) return null;
                r[i] = v.Value;
            }

            return new TargetMatrix(r);
        }

        public TargetMatrix fillResolve(TargetMatrix tm, Card resolveCard, Game g)
        {
            TargetColumn[] r = tm.columns;

            for (int i = 0; i < rules.Length; i++)
            {
                r[i] = rules[i].fillResolveTargets(new ResolveEnv(resolveCard), r[i]);
            }

            return new TargetMatrix(r);
        }

    }


    struct ChooseTargetToolbox
    {
        public Func<Stuff> getTargetable { get; }

        public ChooseTargetToolbox(Func<Stuff> getTargetable)
        {
            this.getTargetable = getTargetable;
        }
    }

    class TargetMatrix
    {
        public TargetColumn[] columns;

        public TargetMatrix(TargetColumn[] columns)
        {
            this.columns = columns;
        }

        public TargetRow[] generateRows()
        {
            int l = columns.Aggregate(1, (current, c) => current*c.targets.Length);
            TargetRow[] r = new TargetRow[l];
            if (l != 1) throw new Exception();

            Targetable[] i1 = new Targetable[columns.Length];
            for (int i = 0; i < columns.Length; i++)
            {
                i1[i] = columns[i].targets[0];
            }

            r[0] = new TargetRow(i1);

            return r;
        }
    }

    interface TargetRule
    {
        TargetColumn? fillCastTargets(ChooseTargetToolbox f);
        TargetColumn fillResolveTargets(ResolveEnv re, TargetColumn c);
    }

    class PryTileRule : TargetRule
    {
        private Func<Tile, bool> filter;

        public PryTileRule(Func<Tile, bool> filter)
        {
            this.filter = filter;
        }

        public TargetColumn? fillCastTargets(ChooseTargetToolbox box)
        {
            while (true)
            {
                Stuff v = box.getTargetable();

                var b = v as ShibbuttonStuff;
                if (b?.option == ButtonOption.Cancel) return null;

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

        public TargetColumn? fillCastTargets(ChooseTargetToolbox box)
        {
            while (true)
            {
                Stuff v = box.getTargetable();

                if (v is ShibbuttonStuff)
                {
                    var b = (ShibbuttonStuff)v;
                    if (b.option == ButtonOption.Cancel) return null;
                }

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
            if (c.targets.Length != 0) throw new Exception();
            switch (rule)
            {
                case Rule.ResolveCard:
                {
                    return new TargetColumn(re.resolveCard);
                }
            }
            throw new Exception();
        }

        public TargetColumn? fillCastTargets(ChooseTargetToolbox box)
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
        public Targetable[] targets;
        
        public TargetColumn(params Targetable[] targets)
        {
            this.targets = targets;
        }
    }
    
    
    interface Targetable
    {
        
    }
}
